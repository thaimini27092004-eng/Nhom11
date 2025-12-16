using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models.VaiTro;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
            {
                return Unauthorized();
            }

            return View();
        }


    }
}