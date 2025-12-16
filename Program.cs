//Program.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories;
using WebsiteBanHang.Repositories.LopThucThi;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi;
using WebsiteBanHang.Repositories.I.QLKhoHang;
using WebsiteBanHang.Repositories.LopThucThi.QLKho;
using WebsiteBanHang.Repositories.EF;
using WebsiteBanHang.Services;
using WebsiteBanHang.Services.HoaDon;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using Microsoft.Extensions.Logging; // Thêm để ghi log
//using WebsiteBanHang.Services.CapNhat;
using WebsiteBanHang.Services.VnPay;
using WebsiteBanHang.Services.AITuVan;
using WebsiteBanHang.Repositories.LopThucThi.QLDanhGiaPhanHoiTraLoi;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;
using WebsiteBanHang.Repositories.LopThucThi.QuanLyHoaDon;
using WebsiteBanHang.Repositories.I.QuanLyThongKe;
using WebsiteBanHang.Repositories.LopThucThi.QuanLyThongKe;
using WebsiteBanHang.Repositories.I.QLNhanVien;
using WebsiteBanHang.Repositories.LopThucThi.QLNhanVien;
using WebsiteBanHang.Repositories.I.QLBanner;
using WebsiteBanHang.Repositories.LopThucThi.QLBanner;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Repositories.LopThucThi.QLTrangThai;
using WebsiteBanHang.Services.GuiEmailTHongBao;
using WebsiteBanHang.Repositories.I.QLGmailThongBao;
using WebsiteBanHang.Repositories.LopThucThi.QLGmailThongBao;

var builder = WebApplication.CreateBuilder(args);
// Thêm dịch vụ logging

// Đặt trước AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddRazorPages(); // Đăng ký Razor Pages
// Cấu hình Identity
builder.Services.AddIdentity<ThongTinNguoiDung, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cho phép đăng nhập ngay, thay vì yêu cầu xác nhận email
    options.Password.RequireDigit = false;          // Không yêu cầu số
    options.Password.RequireLowercase = false;      // Không yêu cầu ký tự thường
    options.Password.RequireUppercase = true;      // Không yêu cầu ký tự in hoa
    options.Password.RequireNonAlphanumeric = true; // Không yêu cầu ký tự đặc biệt
    options.Password.RequiredLength = 6;            // Chỉ yêu cầu độ dài tối thiểu 6
    options.Password.RequiredUniqueChars = 0;       // Không yêu cầu ký tự khác biệt

})
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Đăng ký Google Authentication
builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    
});




builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(); // Đăng ký Controllers
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
// Đăng ký các repository và service
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
//builder.Services.AddTransient<IEmailSender, EmailSender>(); // Thêm dịch vụ email
builder.Services.AddScoped<INguoiDungRepository, EFNguoiDungRepository>();
builder.Services.AddScoped<INguoiDungRepository, EFNguoiDungRepository>();
builder.Services.AddScoped<ISanPhamRepository, EFSanPhamRepository>();
builder.Services.AddScoped<IDanhMucRepository, EFDanhMucRepository>();
builder.Services.AddScoped<IPTTTRepository, EFPTTTRepository>();
builder.Services.AddScoped<IHoaDonRepository, EFHoaDonRepository>();
builder.Services.AddScoped<ITrangThaiRepository, EFTrangThaiRepository>();
builder.Services.AddScoped<IDSAnhRepository, EFDSAnhRepository>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IGioHangRepository, EFGioHangRepository>();
builder.Services.AddScoped<ICTGioHangRepository, EFCTGioHangRepository>();
builder.Services.AddScoped<IHoaDonRepository, EFHoaDonRepository>();
builder.Services.AddScoped<ICTHDRepository, EFCTHDRepository>();
builder.Services.AddScoped<ITonKhoRepository, EFTonKhoRepository>();
builder.Services.AddScoped<IDanhGiaRepository, EFDanhGiaRepository>();
builder.Services.AddScoped<IAnhDGRepository, EFAnhDGRepository>();
builder.Services.AddScoped<IBinhLuanRepository, EFBinhLuanRepository>();
builder.Services.AddScoped<IPhanHoiBinhLuanRepository, EFTraLoiRepository>();
builder.Services.AddScoped<ILikeBinhLuanRepository, EFLikeBinhLuanRepository>();
builder.Services.AddScoped<ILikePhanHoiBinhLuanRepository, EFLikePhanHoiBinhLuanRepository>();
builder.Services.AddScoped<ILikeDanhGiaRepository, EFLikeDanhGiaRepository>();
builder.Services.AddScoped<ILikePhanHoiDanhGiaRepository, EFLikePhanHoiDanhGiaRepository>();
builder.Services.AddScoped<IPhanHoiDanhGiaRepository, EFPhanHoiDanhGiaRepository>();
builder.Services.AddScoped<IKhoRepository, EFKhoRepository>();
builder.Services.AddScoped<IKhachHangRepository, EFKhachHangRepository>();
builder.Services.AddScoped<IQuanLyHoaDonRepository, EFQuanLyHoaDonRepository>();
builder.Services.AddScoped<IThongKeRepository, EFThongKeRepository>();
builder.Services.AddScoped<INhanVienRepository, EFNhanVienRepository>();
builder.Services.AddScoped<IVaiTroNhanVienRepository, EFVaiTroNhanVienRepository>();
builder.Services.AddScoped<IBannerQuangCaoRepository, EFBannerQuangCaoRepository>();
builder.Services.AddScoped<ILichSuTTHDRepository, EFLichSuTTHDRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGmailThongBaoRepository, EFGmailThongBaoRepository>();
builder.Services.AddScoped<GeminiService>();





var app = builder.Build();


// Seeding tài khoản Admin
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ThongTinNguoiDung>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Tạo vai trò Admin
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Tạo tài khoản Admin
    string userName = builder.Configuration["AdminCredentials:UserName"];
    string adminEmail = builder.Configuration["AdminCredentials:Email"];
    string adminPassword = builder.Configuration["AdminCredentials:Password"];
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var user = new ThongTinNguoiDung
        {
            UserName = userName,
            Email = adminEmail,
            HoTen = "Admin User",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
            context.KhachHang.Add(new KhachHang { TenKH = user.HoTen, UserId = user.Id });
            await context.SaveChangesAsync();
        }
    }
}



// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Phải trước UseAuthorization
app.UseAuthorization();


// Định nghĩa middleware chuyển hướng
app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path.Value;

    if (requestPath.Equals("/Identity/Account/Manage", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/CustomAccount/QuanLyTaiKhoan");
        return;
    }
    else if (requestPath.Equals("/Identity/Account/Manage/Email", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/CustomAccount/QuanLyEmail");
        return;
    }
    else if (requestPath.Equals("/Identity/Account/Manage/ChangePassword", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/CustomAccount/DoiMatKhau");
        return;
    }
    else if (requestPath.Equals("/Identity/Account/Manage/TwoFactorAuthentication", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/CustomAccount/XacThucHaiLop");
        return;
    }
    else if (requestPath.Equals("/Identity/Account/Manage/PersonalData", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/CustomAccount/DuLieuCaNhan");
        return;
    }

    await next();
});

// Định nghĩa routing cho Razor Pages và Controllers
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages(); // Ánh xạ Razor Pages, bao gồm trong Areas
    endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();