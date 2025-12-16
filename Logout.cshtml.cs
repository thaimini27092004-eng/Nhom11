
//logout.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
       

        public LogoutModel(SignInManager<ThongTinNguoiDung> signInManager, 
            ILogger<LogoutModel> logger
            )
        {
            _signInManager = signInManager;
            _logger = logger;
            
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            _logger.LogInformation("Bắt đầu quá trình đăng xuất cho người dùng: {UserName}", User?.Identity?.Name);
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Đăng xuất thành công");

            // Trả về JSON thay vì chuyển hướng
            return new JsonResult(new { success = true, message = "Đăng xuất thành công" });
        }

        public IActionResult OnGet()
        {
            return RedirectToAction("Index", "Home");
        }


    }
}