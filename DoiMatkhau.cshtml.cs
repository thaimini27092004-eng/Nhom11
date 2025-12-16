
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.NguoiDung;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace WebsiteBanHang.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class DoiMatKhaudModel : PageModel
    {
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public DoiMatKhaudModel(
            UserManager<ThongTinNguoiDung> userManager, 
            IEmailSender emailSender, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        //Sử dụng System.Security.Cryptography để tạo mã OTP an toàn
        private string GenerateOtp(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var bytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }
            return new string(result);
        }

        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập email hợp lệ." });
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Luôn trả về success để tránh tiết lộ thông tin
                return new JsonResult(new { success = true, message = "Nếu email bạn nhập là đúng, mã xác nhận đã được gửi." });
            }

            //kiểm tra tần suất
            var recentOtps = await _context.MaXacThuc
                .Where(m => m.Email == Input.Email && m.ThoiGianTao > DateTime.UtcNow.AddMinutes(-5))
                .CountAsync();
            if (recentOtps >= 5)
            {
                return new JsonResult(new { success = false, message = "Bạn đã yêu cầu quá nhiều mã xác nhận. Vui lòng thử lại sau 5 phút." });
            }

            // Vô hiệu hóa tất cả mã xác nhận cũ của email này
            var oldOtps = await _context.MaXacThuc
                .Where(m => m.Email == Input.Email && !m.DaSuDung)
                .ToListAsync();
            foreach (var otp in oldOtps)
            {
                otp.DaSuDung = true;
            }

            // Tạo mã xác nhận 8 Ký tự
            var code = GenerateOtp(8); // Tạo mã 8 ký tự
            var expiryTime = DateTime.UtcNow.AddMinutes(3);

            // Lưu mã vào bảng MaXacThuc
            var maXacThuc = new MaXacThuc
            {
                Email = Input.Email,
                MaXacNhan = code,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianHetHan = expiryTime,
                DaSuDung = false
            };
            _context.MaXacThuc.Add(maXacThuc);
            await _context.SaveChangesAsync();

            // Gửi email chứa mã xác nhận
            var emailContent = $"<p>Mã xác nhận của bạn là: <strong>{code}</strong></p><p>Mã này có hiệu lực trong 3 phút.</p>";
            await _emailSender.SendEmailAsync(Input.Email, "Mã xác nhận đặt lại mật khẩu", emailContent);

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostVerifyCodeAsync(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập email và mã xác nhận." });
            }

            // Lấy mã xác nhận mới nhất trong 5 phút
            var maXacThuc = await _context.MaXacThuc
                .Where(m => m.Email == email && m.MaXacNhan == code && !m.DaSuDung && m.ThoiGianHetHan > DateTime.UtcNow.AddMinutes(-5))
                .OrderByDescending(m => m.ThoiGianTao)
                .FirstOrDefaultAsync();


            if (maXacThuc == null)
            { 

                // Lấy mã mới nhất trong 5 phút, chưa dùng, để tăng số lần thử
                var latestOtp = await _context.MaXacThuc
                    .Where(m => m.Email == email && m.DaSuDung == false && m.ThoiGianHetHan > DateTime.UtcNow.AddMinutes(-5))
                    .OrderByDescending(m => m.ThoiGianTao)
                    .FirstOrDefaultAsync();

                if (latestOtp != null)
                {

                    latestOtp.SoLanThu++;
                    if (latestOtp.SoLanThu >= 5)
                    {
                        latestOtp.DaSuDung = true; // Vô hiệu hóa mã
                        await _context.SaveChangesAsync();
                        return new JsonResult(new { success = false, message = "Bạn đã nhập sai mã xác nhận 5 lần. Vui lòng yêu cầu mã mới." });
                    }
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = false, message = $"Mã xác nhận không hợp lệ. Bạn còn {5 - latestOtp.SoLanThu} lần thử." });
                }

                return new JsonResult(new { success = false, message = "Mã đã hết hiệu lực. Vui lòng yêu cầu mã mới." });
            }

            // Mã xác nhận hợp lệ, đánh dấu đã sử dụng
            maXacThuc.DaSuDung = true;
            await _context.SaveChangesAsync();

            var tempToken = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("ResetPasswordToken", tempToken);
            return new JsonResult(new { success = true, token = tempToken });
        }
    }
}