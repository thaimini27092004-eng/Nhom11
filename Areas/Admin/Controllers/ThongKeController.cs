using System.Data;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;
using WebsiteBanHang.Repositories.I.QuanLyThongKe;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
       [Area("Admin")]

    public class ThongKeController : Controller
    {

        private readonly ApplicationDbContext _context; // Thêm để truy cập trực tiếp vào DbContext
        private readonly IWebHostEnvironment _environment;
        private readonly IThongKeRepository _thongkeRepository;

        public ThongKeController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IThongKeRepository thongKeRepository
            )
        {
            _context = context;
            _environment = environment;
            _thongkeRepository = thongKeRepository;

        }

        public IActionResult Index()
        {
          

            var count_SanPham = _context.SanPham.Count();
            var count_DanhMuc = _context.DanhMuc.Count();
            var count_HoaDon = _context.HoaDon.Count();
            var count_User = _context.KhachHang.Count();
            var count_Kho = _context.Kho.Count();
            var count_PTTT = _context.PTTT.Count();
            var count_VaiTro = _context.Roles.Count();
            ViewBag.CountSanPham = count_SanPham;
            ViewBag.CountDanhMuc = count_DanhMuc;
            ViewBag.CountHoaDon = count_HoaDon;
            ViewBag.CountUser = count_User;
            ViewBag.CountKho = count_Kho;
            ViewBag.ContPTTT = count_PTTT;
            ViewBag.ContVaiTro = count_VaiTro;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChartData(DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = await _thongkeRepository.GetThongKeAsync(startDate, endDate);
            return Json(data);
        }
        /*
        [HttpPost]
        public IActionResult GetChartData(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.ThongKe.AsQueryable();//truy vấn động

            // Nếu có startDate và endDate, thêm điều kiện lọc
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(s => s.NgayDatHang >= startDate.Value && s.NgayDatHang <= endDate.Value);
            }

            var data = query.Select(s => new
            {
                ngayDatHang = s.NgayDatHang.ToString("yyyy-MM-dd"),
                soluongBan = s.SLBan,
                soLuongDonHang = s.SLDonHang,
                doanhThu = s.DoanhThu,
                loiNhuan = s.LoiNhuan
            }).ToList();

            return Json(data);
        }

        
        [HttpPost]
        public async Task<IActionResult> GetCharData()
        {
            var data = _context.ThongKe.Select(s => new
            {
                ngayDatHang = s.NgayDatHang.ToString("yyyy-MM-dd"),
                soluongBan = s.SLBan,
                soLuongDonHang = s.SLDonHang,
                doanhThu = s.DoanhThu,
                loiNhuan = s.LoiNhuan

            }).ToList();

            return Json(data);
        }

        [HttpPost]
        [Route("GetCharDataBySelect")]
        public IActionResult  GetCharDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = _context.ThongKe
                .Where(s=> s.NgayDatHang >= startDate && s.NgayDatHang <= endDate)
                .Select(s => new
            {
                ngayDatHang = s.NgayDatHang.ToString("yyyy-MM-dd"),
                soluongBan = s.SLBan,
                soLuongDonHang = s.SLDonHang,
                doanhThu = s.DoanhThu,
                loiNhuan = s.LoiNhuan

            }).ToList();

            return Json(data);
        }
        */
    }
}
