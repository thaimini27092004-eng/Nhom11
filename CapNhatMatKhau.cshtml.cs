
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class CapNhatMatKhauModel : PageModel
    {
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly ILogger<CapNhatMatKhauModel> _logger;

        public CapNhatMatKhauModel(UserManager<ThongTinNguoiDung> userManager, 
            ILogger<CapNhatMatKhauModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Mật khẩu không được trống")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Xác nhận mật khẩu không được trống")]
            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; }
        }

        // Biến riêng để lưu email từ query string
        public string EmailFromQuery { get; set; }

        public IActionResult OnGet(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToPage("./DoiMatKhau");
            }

            var sessionToken = HttpContext.Session.GetString("ResetPasswordToken");
            if (sessionToken != token)
            {
                return RedirectToPage("./DoiMatKhau");
            }

            EmailFromQuery = email;
            Input = new InputModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            EmailFromQuery = email;
            if (string.IsNullOrEmpty(EmailFromQuery))
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy email để cập nhật mật khẩu.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                // Log lỗi validate để debug
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning($"Validation error for {error.Key}: {err.ErrorMessage}");
                    }
                }
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(EmailFromQuery);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản với email này.");
                _logger.LogWarning($"Không tìm thấy tài khoản với email: {EmailFromQuery}");
                return Page();
            }

            // Đặt lại mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Mật khẩu của tài khoản {EmailFromQuery} đã được cập nhật thành công.");
                return RedirectToPage("./Login", new { message = "Mật khẩu đã được cập nhật thành công. Vui lòng đăng nhập lại." });
            }

            // Nếu thất bại, hiển thị lỗi chi tiết
            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordRequiresUpper")
                {
                    ModelState.AddModelError("Input.NewPassword", "Mật khẩu phải chứa ít nhất một ký tự in hoa.");
                }
                else if (error.Code == "PasswordRequiresNonAlphanumeric")
                {
                    ModelState.AddModelError("Input.NewPassword", "Mật khẩu phải chứa ít nhất một ký tự đặc biệt (ví dụ: !@#$%).");
                }
                else if (error.Code == "PasswordTooShort")
                {
                    ModelState.AddModelError("Input.NewPassword", "Mật khẩu phải có ít nhất 6 ký tự.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật mật khẩu. Vui lòng thử lại.");
                }
                _logger.LogError($"Lỗi khi cập nhật mật khẩu cho {EmailFromQuery}: {error.Description}");
            }

            return Page();
        }
    }
}