using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories;
using WebsiteBanHang.Repositories.I;

public class AccountController : Controller
{
    private readonly UserManager<ThongTinNguoiDung> _userManager;
    private readonly SignInManager<ThongTinNguoiDung> _signInManager;
    private readonly INguoiDungRepository _nguoiDungRepository;

    public AccountController(
        UserManager<ThongTinNguoiDung> userManager,
        SignInManager<ThongTinNguoiDung> signInManager,
        INguoiDungRepository nguoiDungRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _nguoiDungRepository = nguoiDungRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            // Sử dụng repository để tìm kiếm người dùng bằng email, user hoặc SDT
            var user = await _nguoiDungRepository.FindByEmailOrSDTOrUserNameAsync(model.LoginInput);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Kiểm tra mật khẩu và đăng nhập
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        return View(model);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new ThongTinNguoiDung
            {
                UserName = model.Email,
                Email = model.Email,
                SDT = model.SDT,
                HoTen = model.HoTen,
                DiaChi = model.DiaChi,
                NgaySinh = model.NgaySinh
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            AddErrors(result);
        }

        return View(model);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}