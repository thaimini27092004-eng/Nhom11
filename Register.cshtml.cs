using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Models;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly IUserStore<ThongTinNguoiDung> _userStore;
        private readonly IUserEmailStore<ThongTinNguoiDung> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ThongTinNguoiDung> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ThongTinNguoiDung> userStore,
            SignInManager<ThongTinNguoiDung> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Họ và tên không được trống")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Tên đăng nhập không được trống")]
            [StringLength(50, ErrorMessage = "Tên đăng nhập phải từ {2} đến {1} ký tự", MinimumLength = 3)]
            [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số, gạch dưới hoặc gạch nối")]
            [Display(Name = "UserName")]
            public string UserName { get; set; }


            [Required(ErrorMessage = "Email không được trống")]
            [EmailAddress(ErrorMessage = "Email phải có định dạng hợp lệ và chứa ký tự '@'")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được trống")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải dài ít nhất {2} và tối đa {1} ký tự", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
            public string ConfirmPassword { get; set; }

            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
            }

            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // Hàm tạo mã OTP ngẫu nhiên
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

        // Xử lý gửi mã OTP
        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập thông tin hợp lệ." });
            }

            // Kiểm tra email đã tồn tại
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                return new JsonResult(new { success = false, message = "Email này đã được sử dụng." });
            }

            // Kiểm tra tên đăng nhập đã tồn tại
            var existingUserByName = await _userManager.FindByNameAsync(Input.UserName);
            if (existingUserByName != null)
            {
                return new JsonResult(new { success = false, message = "Tên đăng nhập này đã được sử dụng." });
            }

            // Kiểm tra tần suất gửi mã
            var recentOtps = await _context.MaXacThuc
                .Where(m => m.Email == Input.Email && m.ThoiGianTao > DateTime.UtcNow.AddMinutes(-5))
                .CountAsync();
            if (recentOtps >= 5)
            {
                return new JsonResult(new { success = false, message = "Bạn đã yêu cầu quá nhiều mã xác nhận. Vui lòng thử lại sau 5 phút." });
            }

            // Vô hiệu hóa các mã OTP cũ
            var oldOtps = await _context.MaXacThuc
                .Where(m => m.Email == Input.Email && !m.DaSuDung)
                .ToListAsync();
            foreach (var otp in oldOtps)
            {
                otp.DaSuDung = true;
            }

            // Tạo mã OTP
            var code = GenerateOtp(8);
            var expiryTime = DateTime.UtcNow.AddMinutes(3);

            // Lưu mã OTP
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

            // Gửi email
            var emailContent = $"<p>Mã xác nhận đăng ký của bạn là: <strong>{code}</strong></p><p>Mã này có hiệu lực trong 3 phút.</p>";
            await _emailSender.SendEmailAsync(Input.Email, "Mã xác nhận đăng ký tài khoản", emailContent);

            return new JsonResult(new { success = true });
        }

        // Xử lý xác nhận mã OTP và tạo tài khoản
        public async Task<IActionResult> OnPostVerifyCodeAsync(string email, string code, InputModel input)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập email và mã xác nhận." });
            }

            // Kiểm tra mã OTP
            var maXacThuc = await _context.MaXacThuc
                .Where(m => m.Email == email && m.MaXacNhan == code && !m.DaSuDung && m.ThoiGianHetHan > DateTime.UtcNow)
                .OrderByDescending(m => m.ThoiGianTao)
                .FirstOrDefaultAsync();

            if (maXacThuc == null)
            {
                var latestOtp = await _context.MaXacThuc
                    .Where(m => m.Email == email && !m.DaSuDung && m.ThoiGianHetHan > DateTime.UtcNow.AddMinutes(-5))
                    .OrderByDescending(m => m.ThoiGianTao)
                    .FirstOrDefaultAsync();

                if (latestOtp != null)
                {
                    latestOtp.SoLanThu++;
                    if (latestOtp.SoLanThu >= 5)
                    {
                        latestOtp.DaSuDung = true;
                        await _context.SaveChangesAsync();
                        return new JsonResult(new { success = false, message = "Bạn đã nhập sai mã xác nhận 5 lần. Vui lòng yêu cầu mã mới." });
                    }
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = false, message = $"Mã xác nhận không hợp lệ. Bạn còn {5 - latestOtp.SoLanThu} lần thử." });
                }

                return new JsonResult(new { success = false, message = "Mã đã hết hiệu lực. Vui lòng yêu cầu mã mới." });
            }
            // Kiểm tra tên đăng nhập đã tồn tại
            var existingUserByName = await _userManager.FindByNameAsync(Input.UserName);
            if (existingUserByName != null)
            {
                return new JsonResult(new { success = false, message = "Tên đăng nhập này đã được sử dụng." });
            }

            // Mã OTP hợp lệ, đánh dấu đã sử dụng
            maXacThuc.DaSuDung = true;
            await _context.SaveChangesAsync();

            // Tạo tài khoản
            var user = CreateUser();
            user.HoTen = input.FullName;
            user.EmailConfirmed = true;
            user.UserName = input.UserName;
            await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Người dùng đã tạo tài khoản mới với mật khẩu.");
                if (!string.IsNullOrEmpty(input.Role))
                {
                    await _userManager.AddToRoleAsync(user, input.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                }

                // Tạo bản ghi KhachHang
                var khachHang = new KhachHang
                {
                    TenKH = input.FullName,
                    DiaChiKH = null,
                    DTKH = null,
                    UserId = user.Id
                };
                _context.KhachHang.Add(khachHang);
                await _context.SaveChangesAsync();

                // Đăng nhập
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new JsonResult(new { success = true, redirectUrl = Url.Content("~/") });
            }

            // Nếu tạo tài khoản thất bại
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new JsonResult(new { success = false, message = string.Join("; ", errors) });
        }

        private ThongTinNguoiDung CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ThongTinNguoiDung>();
            }
            catch
            {
                throw new InvalidOperationException($"Không thể tạo instance của '{nameof(ThongTinNguoiDung)}'. " +
                    $"Hãy đảm bảo '{nameof(ThongTinNguoiDung)}' không phải là abstract class và có constructor không tham số.");
            }
        }

        private IUserEmailStore<ThongTinNguoiDung> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("UI mặc định yêu cầu user store hỗ trợ email.");
            }
            return (IUserEmailStore<ThongTinNguoiDung>)_userStore;
        }
    }
}