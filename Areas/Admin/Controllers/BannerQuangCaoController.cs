using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models.QuanLyBanner;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I.QLBanner;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerQuangCaoController : Controller
    {
        private readonly IBannerQuangCaoRepository _bannerRepository;
        private readonly ApplicationDbContext _context;

        public BannerQuangCaoController(IBannerQuangCaoRepository bannerRepository, ApplicationDbContext context)
        {
            _bannerRepository = bannerRepository;
            _context = context;
        }

        // Hiển thị danh sách banner và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi", string sapXepMoTa = "macDinh")
        {
            var banners = (await _bannerRepository.GetAllAsync()).ToList();

            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;
            ViewData["sapXepMoTa"] = sapXepMoTa;

            // Lọc theo trạng thái
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    banners = banners.Where(b => !b.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    banners = banners.Where(b => b.TTHienThi).ToList();
                    break;
            }

            // Sắp xếp theo mô tả
            switch (sapXepMoTa)
            {
                case "aDenZ":
                    banners = banners.OrderBy(b => b.MoTa).ToList();
                    break;
                case "zDenA":
                    banners = banners.OrderByDescending(b => b.MoTa).ToList();
                    break;
                default:
                    break;
            }

            // Tìm kiếm
            if (string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                return View(banners);
            }

            var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var ketQuaTimKiem = new List<BannerQuangCao>();
            var diemSo = new List<int>();

            for (int i = 0; i < banners.Count; i++)
            {
                int diem = TinhSoTuKhop(banners[i], tuKhoaArray);
                if (diem > 0)
                {
                    ketQuaTimKiem.Add(banners[i]);
                    diemSo.Add(diem);
                }
            }

            for (int i = 0; i < ketQuaTimKiem.Count - 1; i++)
            {
                for (int j = i + 1; j < ketQuaTimKiem.Count; j++)
                {
                    if (diemSo[i] < diemSo[j])
                    {
                        var temp = ketQuaTimKiem[i];
                        ketQuaTimKiem[i] = ketQuaTimKiem[j];
                        ketQuaTimKiem[j] = temp;
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            if (ketQuaTimKiem.Count > 0)
            {
                return View(ketQuaTimKiem);
            }
            else
            {
                ViewBag.KetQuaTimKiem = "Không có banner liên quan.";
                var goiY = banners.Take(3).ToList();
                return View(goiY);
            }
        }

        // Action gợi ý tìm kiếm
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var banners = (await _bannerRepository.GetAllAsync()).ToList();

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    banners = banners.Where(b => !b.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    banners = banners.Where(b => b.TTHienThi).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            for (int i = 0; i < banners.Count; i++)
            {
                int diem = TinhSoKyTuKhop(banners[i], tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new
                    {
                        maAQC = banners[i].MaAQC,
                        moTa = banners[i].MoTa,
                        urlAnh = banners[i].UrlAnh
                    });
                    diemSo.Add(diem);
                }
            }

            for (int i = 0; i < goiY.Count - 1; i++)
            {
                for (int j = i + 1; j < goiY.Count; j++)
                {
                    if (diemSo[i] < diemSo[j])
                    {
                        var temp = goiY[i];
                        goiY[i] = goiY[j];
                        goiY[j] = temp;
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            var ketQua = new List<object>();
            for (int i = 0; i < 3 && i < goiY.Count; i++)
            {
                ketQua.Add(goiY[i]);
            }

            return Json(ketQua);
        }

        // Hàm tính số từ khớp
        private int TinhSoTuKhop(BannerQuangCao banner, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var moTa = (" " + (banner.MoTa?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (moTa.Contains(tuKhoa))
                    soTuKhop += 1;
            }
            return soTuKhop;
        }

        // Hàm tính số ký tự khớp
        private int TinhSoKyTuKhop(BannerQuangCao banner, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var moTa = banner.MoTa?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (moTa.Contains(tu))
                    soKyTuKhop += tu.Length;
            }
            return soKyTuKhop;
        }

        // Hiển thị form thêm banner mới
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Add()
        {
            return View();
        }


        // Hiển thị thông tin chi tiết banner
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Display(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // Xử lý thêm banner
        [HttpPost]
        public async Task<IActionResult> Add(BannerQuangCao banner, IFormFile urlAnh)
        {
            ModelState.Remove("UrlAnh"); // Loại bỏ validation cho UrlAnh

            if (urlAnh == null || urlAnh.Length == 0)
            {
                ModelState.AddModelError("UrlAnh", "Vui lòng chọn ảnh banner.");
            }

            if (ModelState.IsValid)
            {
                if (urlAnh != null && urlAnh.Length > 0)
                {
                    banner.UrlAnh = await SaveImage(urlAnh);
                }

                banner.TTHienThi = true;
                await _bannerRepository.AddAsync(banner);
                return RedirectToAction(nameof(Index));
            }

            return View(banner);
        }

        // Hiển thị form cập nhật banner
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Update(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // Xử lý cập nhật banner
        [HttpPost]
        public async Task<IActionResult> Update(int id, BannerQuangCao banner, IFormFile urlAnh)
        {
            ModelState.Remove("UrlAnh");
            if (id != banner.MaAQC)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingBanner = await _bannerRepository.GetByIdAsync(id);
                if (existingBanner == null)
                {
                    return NotFound();
                }

                // Xử lý ảnh
                if (urlAnh != null && urlAnh.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingBanner.UrlAnh))
                    {
                        var oldPath = Path.Combine("wwwroot", existingBanner.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    existingBanner.UrlAnh = await SaveImage(urlAnh);
                }

                existingBanner.MoTa = banner.MoTa;
                existingBanner.UrlDichDen = banner.UrlDichDen;
                existingBanner.TTHienThi = banner.TTHienThi;

                await _bannerRepository.UpdateAsync(existingBanner);
                return RedirectToAction(nameof(Index));
            }

            return View(banner);
        }

        // Hiển thị form xác nhận xóa banner
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // Xử lý xóa banner
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner != null)
            {
                if (!string.IsNullOrEmpty(banner.UrlAnh))
                {
                    var imagePath = Path.Combine("wwwroot", banner.UrlAnh.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                await _bannerRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        // Xử lý xóa nhiều banner
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhieuBanner(List<int> selectedBannerMaAQC)
        {
            if (selectedBannerMaAQC == null || selectedBannerMaAQC.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một banner để xóa.";
                return RedirectToAction("Index");
            }

            for (int i = 0; i < selectedBannerMaAQC.Count; i++)
            {
                var banner = await _bannerRepository.GetByIdAsync(selectedBannerMaAQC[i]);
                if (banner != null)
                {
                    if (!string.IsNullOrEmpty(banner.UrlAnh))
                    {
                        var imagePath = Path.Combine("wwwroot", banner.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    await _bannerRepository.DeleteAsync(selectedBannerMaAQC[i]);
                }
            }

            TempData["Message"] = $"Đã xóa {selectedBannerMaAQC.Count} banner.";
            return RedirectToAction("Index");
        }

        // Thay đổi trạng thái hiển thị
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool status, string chonTrangThai)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
            {
                return Json(new { success = false, message = "Banner không tồn tại." });
            }

            banner.TTHienThi = status;
            await _bannerRepository.UpdateAsync(banner);

            return Json(new { success = true });
        }

        // Cập nhật trạng thái nhiều banner
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(string selectedBannerMaAQC, bool newTrangThai)
        {
            if (string.IsNullOrEmpty(selectedBannerMaAQC))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một banner để cập nhật." });
            }

            var danhSachMaAQC = new List<int>();
            var mangId = selectedBannerMaAQC.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int maAQC))
                {
                    danhSachMaAQC.Add(maAQC);
                }
            }

            if (danhSachMaAQC.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách banner không hợp lệ." });
            }

            var updateBanner = new List<object>();
            for (int i = 0; i < danhSachMaAQC.Count; i++)
            {
                var banner = await _bannerRepository.GetByIdAsync(danhSachMaAQC[i]);
                if (banner != null)
                {
                    var trangThaiCu = banner.TTHienThi;
                    banner.TTHienThi = newTrangThai;
                    await _bannerRepository.UpdateAsync(banner);
                    updateBanner.Add(new
                    {
                        maAQC = banner.MaAQC,
                        moTa = banner.MoTa,
                        trangThaiCu = trangThaiCu ? "Bật" : "Tắt",
                        trangThaiMoi = newTrangThai ? "Bật" : "Tắt"
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái cho {danhSachMaAQC.Count} banner.",
                updatedBanner = updateBanner
            });
        }

        // Hàm lưu ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine("wwwroot/images/banner", fileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/banner/" + fileName;
        }
    }
}