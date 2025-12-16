// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories.I;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace WebsiteBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginModel> _logger;

        public AccountController(
            SignInManager<ThongTinNguoiDung> signInManager,
            UserManager<ThongTinNguoiDung> userManager,
            ApplicationDbContext context,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // Hàm bắt đầu đăng nhập Google
        public async Task LoginByGoogle(string returnUrl = null)
        {
            // Nói với hệ thống: "Chuyển người dùng đến trang đăng nhập Google"
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl = returnUrl ?? "/" })
            });
        }

        // Hàm xử lý sau khi Google trả về thông tin người dùng
        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            // Lấy thông tin từ Google (như email, tên)
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
            {
                // Nếu không lấy được thông tin, báo lỗi
                TempData["Error"] = "Đăng nhập Google thất bại!";
                return RedirectToAction("Index", "Home");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claims => new
            {
                claims.Issuer,
                claims.OriginalIssuer,
                claims.Type,
                claims.Value
            });
            //return json(claims);
            // Lấy email từ Google
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string emailName = email.Split('@')[0];
            // Kiểm tra xem email có tồn tại không
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Không lấy được email từ Google!";
                return RedirectToAction("Index", "Home");
            }

            // Tìm người dùng trong hệ thống dựa vào email
            var timNguoiDung = await _userManager.FindByEmailAsync(email);

            // Nếu người dùng chưa có tài khoản (user == null)
            if (timNguoiDung == null)
            {
                // Tạo tài khoản mới
                timNguoiDung = new ThongTinNguoiDung
                {
                    UserName = emailName, // Dùng email bỏ đuôi @ làm tên đăng nhập
                    Email = email,
                    HoTen = fullName ?? "Khách Google", // Nếu không có tên, đặt mặc định
                    EmailConfirmed = true // Google đã xác nhận email, nên đặt là true
                };

                // Tạo tài khoản trong hệ thống từ thông ThongTinNguoiDung
                var taoMoi = await _userManager.CreateAsync(timNguoiDung);
                if (!taoMoi.Succeeded)
                {
                    // Nếu tạo thất bại, báo lỗi
                    TempData["Error"] = "Không thể tạo tài khoản mới!";
                    return RedirectToAction("Index", "Home");
                }

                // Gán vai trò "Customer" cho người dùng mới
                await _userManager.AddToRoleAsync(timNguoiDung, SD.Role_Customer);

                // Thêm vào bảng KhachHang
                var khachHang = new KhachHang
                {
                    TenKH = timNguoiDung.HoTen,
                    UserId = timNguoiDung.Id // Liên kết với tài khoản vừa tạo
                };
                _context.KhachHang.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            // Đăng nhập người dùng vào hệ thống
            await _signInManager.SignInAsync(timNguoiDung, isPersistent: false);

            // Báo thành công và chuyển hướng
            TempData["Success"] = "Đăng nhập thành công!";
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}

