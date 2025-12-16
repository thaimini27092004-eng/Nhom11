using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Areas.Identity.Pages.CustomAccount
{
    [Authorize]
    public class DuLieuCaNhanModel : PageModel
    {
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;

        public DuLieuCaNhanModel(UserManager<ThongTinNguoiDung> userManager, SignInManager<ThongTinNguoiDung> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadPersonalDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            var personalData = new Dictionary<string, string>
            {
                { "UserName", user.UserName },
                { "Email", user.Email },
                { "HoTen", user.HoTen ?? string.Empty }
            };

            var fileContent = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(personalData));
            var fileName = "DuLieuCaNhan.json";

            return File(fileContent, "application/json", fileName);
        }

        public async Task<IActionResult> OnPostDeletePersonalDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);
            StatusMessage = "Tài khoản của bạn đã được xóa.";
            return Redirect("~/");
        }
    }
}