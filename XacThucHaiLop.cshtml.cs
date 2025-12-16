using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Areas.Identity.Pages.CustomAccount
{
    [Authorize]
    public class XacThucHaiLopModel : PageModel
    {
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;

        public XacThucHaiLopModel(UserManager<ThongTinNguoiDung> userManager, SignInManager<ThongTinNguoiDung> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public bool Is2FaEnabled { get; set; }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
            }

            Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (Is2FaEnabled)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                StatusMessage = "Xác thực hai lớp đã được tắt.";
            }
            else
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                StatusMessage = "Xác thực hai lớp đã được bật.";
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }
    }
}