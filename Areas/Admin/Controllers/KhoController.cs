using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Models.QLTonKho;
using WebsiteBanHang.Repositories.I.QLKhoHang;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhoController : Controller
    {
        private readonly ITonKhoRepository _tonkhoRepository;
        private readonly IKhoRepository _khoRepository;

        public KhoController(ITonKhoRepository tonkhoRepository, IKhoRepository khoRepository)
        {
            _tonkhoRepository = tonkhoRepository;
            _khoRepository = khoRepository;
        }

        // Hiển thị danh sách kho và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi")
        {
            var khos = (await _khoRepository.GetAllAsync()).ToList();
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;

            // Lọc theo trạng thái
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    khos = khos.Where(c => !c.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    khos = khos.Where(c => c.TTHienThi).ToList();
                    break;
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ketQuaTimKiem = new List<Kho>();
                var diemSo = new List<int>();

                foreach (var kho in khos)
                {
                    int diem = TinhSoTuKhop(kho, tuKhoaArray);
                    if (diem > 0)
                    {
                        ketQuaTimKiem.Add(kho);
                        diemSo.Add(diem);
                    }
                }

                // Sắp xếp theo điểm giảm dần
                for (int i = 0; i < ketQuaTimKiem.Count - 1; i++)
                {
                    for (int j = i + 1; j < ketQuaTimKiem.Count; j++)
                    {
                        if (diemSo[i] < diemSo[j])
                        {
                            var tempKho = ketQuaTimKiem[i];
                            ketQuaTimKiem[i] = ketQuaTimKiem[j];
                            ketQuaTimKiem[j] = tempKho;
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
                    ViewBag.KetQuaTimKiem = "Không tìm thấy kho liên quan.";
                    return View(khos.Take(3).ToList());
                }
            }

            return View(khos);
        }

        // Tính số từ khớp cho tìm kiếm
        private int TinhSoTuKhop(Kho kho, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var tenKho = (" " + (kho.TenKho?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (tenKho.Contains(tuKhoa))
                    soTuKhop += 2;
            }
            return soTuKhop;
        }

        // Gợi ý tìm kiếm
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var khos = (await _khoRepository.GetAllAsync()).ToList();

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    khos = khos.Where(c => !c.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    khos = khos.Where(c => c.TTHienThi).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            foreach (var kho in khos)
            {
                int diem = TinhSoKyTuKhop(kho, tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new { maKho = kho.MaKho, tenKho = kho.TenKho });
                    diemSo.Add(diem);
                }
            }

            for (int i = 0; i < goiY.Count - 1; i++)
            {
                for (int j = i + 1; j < goiY.Count; j++)
                {
                    if (diemSo[i] < diemSo[j])
                    {
                        var tempGoiY = goiY[i];
                        goiY[i] = goiY[j];
                        goiY[j] = tempGoiY;
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            return Json(goiY.Take(3));
        }

        // Tính số ký tự khớp cho gợi ý
        private int TinhSoKyTuKhop(Kho kho, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var tenKho = kho.TenKho?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (tenKho.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
            }
            return soKyTuKhop;
        }

        // Kiểm tra kho có tồn kho liên quan không
        [HttpPost]
        public async Task<IActionResult> CheckKhoInTonKho(List<int> selectedKhoMaKho)
        {
            var allTonKhos = await _tonkhoRepository.GetAllAsync();
            var result = new List<object>();

            foreach (var maKho in selectedKhoMaKho)
            {
                var kho = await _khoRepository.GetByIdAsync(maKho);
                bool hasTonKho = allTonKhos.Any(t => t.MaKho == maKho);
                result.Add(new { maKho = kho.MaKho, tenKho = kho.TenKho, hasTonKho });
            }

            return Json(result);
        }

        // Xử lý xóa hoặc ẩn nhiều kho
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedKhoMaKho, bool isDelete = false)
        {
            if (selectedKhoMaKho == null || selectedKhoMaKho.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một kho để xử lý.";
                return RedirectToAction("Index");
            }

            var allTonKhos = await _tonkhoRepository.GetAllAsync();
            var khoToDelete = new List<int>();
            var khoToHide = new List<int>();

            foreach (var maKho in selectedKhoMaKho)
            {
                bool hasTonKho = allTonKhos.Any(t => t.MaKho == maKho);
                if (hasTonKho)
                    khoToHide.Add(maKho);
                else
                    khoToDelete.Add(maKho);
            }

            if (isDelete)
            {
                foreach (var id in khoToDelete)
                    await _khoRepository.DeleteAsync(id);

                foreach (var id in khoToHide)
                {
                    var kho = await _khoRepository.GetByIdAsync(id);
                    if (kho != null)
                    {
                        kho.TTHienThi = false;
                        await _khoRepository.UpdateAsync(kho);
                    }
                }

                if (khoToDelete.Count > 0)
                    TempData["Message"] = $"Đã xóa {khoToDelete.Count} kho không có tồn kho.";
                if (khoToHide.Count > 0)
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã ẩn {khoToHide.Count} kho có tồn kho.";
            }
            else
            {
                foreach (var id in selectedKhoMaKho)
                {
                    var kho = await _khoRepository.GetByIdAsync(id);
                    if (kho != null)
                    {
                        kho.TTHienThi = false;
                        await _khoRepository.UpdateAsync(kho);
                    }
                }
                TempData["Message"] = $"Đã ẩn {selectedKhoMaKho.Count} kho.";
            }

            return RedirectToAction("Index");
        }

        // Thay đổi trạng thái hiển thị
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int maKho, bool status)
        {
            var kho = await _khoRepository.GetByIdAsync(maKho);
            if (kho == null)
                return Json(new { success = false, message = "Kho không tồn tại." });

            kho.TTHienThi = status;
            await _khoRepository.UpdateAsync(kho);

            return Json(new { success = true });
        }

        // Hiển thị chi tiết kho
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Display(int maKho)
        {
            var kho = await _khoRepository.GetByIdAsync(maKho);
            if (kho == null)
                return NotFound();
            return View(kho);
        }

        // Hiển thị form thêm kho
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm kho
        [HttpPost]
        public async Task<IActionResult> Add(Kho kho)
        {
            if (ModelState.IsValid)
            {
                await _khoRepository.AddAsync(kho);
                return RedirectToAction(nameof(Index));
            }
            return View(kho);
        }

        // Hiển thị form sửa kho
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int maKho)
        {
            var kho = await _khoRepository.GetByIdAsync(maKho);
            if (kho == null)
                return NotFound();
            return View(kho);
        }

        // Xử lý sửa kho
        [HttpPost]
        public async Task<IActionResult> Update(int maKho, Kho kho)
        {
            if (maKho != kho.MaKho)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _khoRepository.UpdateAsync(kho);
                return RedirectToAction(nameof(Index));
            }
            return View(kho);
        }

        // Hiển thị form xác nhận xóa kho
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int maKho)
        {
            var kho = await _khoRepository.GetByIdAsync(maKho);
            if (kho == null)
                return NotFound();
            return View(kho);
        }

        // Xử lý xóa kho
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int maKho)
        {
            var kho = await _khoRepository.GetByIdAsync(maKho);
            if (kho != null)
                await _khoRepository.DeleteAsync(maKho);
            return RedirectToAction(nameof(Index));
        }

        // Xem chi tiết tồn kho theo kho
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XemChiTietTonKhoTheoKho(List<int> selectedKhoMaKho)
        {
            if (selectedKhoMaKho == null || selectedKhoMaKho.Count == 0)
            {
                return RedirectToAction("Index");
            }

            // Lấy kho đầu tiên trong danh sách được chọn
            int chonKho = selectedKhoMaKho[0];

            return RedirectToAction("Index", "SanPham", new
            {
                tuKhoaTimKiem = "",
                chonTrangThai = "tatCa",
                chonDanhMuc = "",
                chonKho = chonKho
            });
        }
    }
}