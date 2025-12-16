using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using System.IO;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TrangThaiController : Controller
    {
        private readonly IHoaDonRepository _hoadonRepository;
        private readonly ITrangThaiRepository _trangthaiRepository;
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;

        public TrangThaiController(IHoaDonRepository hoadonRepository, ITrangThaiRepository trangthaiRepository,
            ILichSuTTHDRepository lichSuTTHDRepository)
        {
            _hoadonRepository = hoadonRepository;
            _trangthaiRepository = trangthaiRepository;
            _lichSuTTHDRepository = lichSuTTHDRepository;
        }

        // Hiển thị danh sách trạng thái và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi")
        {
            var trangThais = (await _trangthaiRepository.GetAllAsync()).ToList();
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    trangThais = trangThais.Where(t => !t.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    trangThais = trangThais.Where(t => t.TTHienThi).ToList();
                    break;
            }

            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ketQuaTimKiem = new List<TrangThai>();
                var diemSo = new List<int>();

                foreach (var trangThai in trangThais)
                {
                    int diem = TinhSoTuKhop(trangThai, tuKhoaArray);
                    if (diem > 0)
                    {
                        ketQuaTimKiem.Add(trangThai);
                        diemSo.Add(diem);
                    }
                }

                for (int i = 0; i < ketQuaTimKiem.Count - 1; i++)
                {
                    for (int j = i + 1; j < ketQuaTimKiem.Count; j++)
                    {
                        if (diemSo[i] < diemSo[j])
                        {
                            var tempTrangThai = ketQuaTimKiem[i];
                            ketQuaTimKiem[i] = ketQuaTimKiem[j];
                            ketQuaTimKiem[j] = tempTrangThai;
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
                    ViewBag.KetQuaTimKiem = "Không tìm thấy trạng thái liên quan.";
                    return View(trangThais.Take(3).ToList());
                }
            }

            return View(trangThais);
        }

        private int TinhSoTuKhop(TrangThai trangThai, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var tenTT = (" " + (trangThai.TenTT?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (tenTT.Contains(tuKhoa))
                    soTuKhop += 2;
            }
            return soTuKhop;
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var trangThais = (await _trangthaiRepository.GetAllAsync()).ToList();

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    trangThais = trangThais.Where(t => !t.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    trangThais = trangThais.Where(t => t.TTHienThi).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            foreach (var trangThai in trangThais)
            {
                int diem = TinhSoKyTuKhop(trangThai, tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new { maTT = trangThai.MaTT, tenTT = trangThai.TenTT, urlAnh = trangThai.UrlAnh }); // Thêm UrlAnh
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

        private int TinhSoKyTuKhop(TrangThai trangThai, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var tenTT = trangThai.TenTT?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (tenTT.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
            }
            return soKyTuKhop;
        }

        [HttpPost]
        public async Task<IActionResult> CheckTrangThaiInHoaDon(List<int> selectedTrangThaiMaTT)
        {
            var allLichSu = await _lichSuTTHDRepository.GetAllAsync();
            var result = new List<object>();

            foreach (var maTT in selectedTrangThaiMaTT)
            {
                var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
                bool hasHoaDon = allLichSu.Any(ls => ls.MaTT == maTT);
                result.Add(new { maTT = trangThai.MaTT, tenTT = trangThai.TenTT, hasHoaDon });
            }

            return Json(result);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedTrangThaiMaTT, bool isDelete = false)
        {
            if (selectedTrangThaiMaTT == null || selectedTrangThaiMaTT.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một trạng thái để xử lý.";
                return RedirectToAction("Index");
            }

            var allLichSu = await _lichSuTTHDRepository.GetAllAsync();
            var trangThaiToDelete = new List<int>();
            var trangThaiToHide = new List<int>();

            foreach (var maTT in selectedTrangThaiMaTT)
            {
                bool hasHoaDon = allLichSu.Any(h => h.MaTT == maTT);
                if (hasHoaDon)
                    trangThaiToHide.Add(maTT);
                else
                    trangThaiToDelete.Add(maTT);
            }

            if (isDelete)
            {
                foreach (var id in trangThaiToDelete)
                {
                    var trangThai = await _trangthaiRepository.GetByIdAsync(id);
                    if (trangThai != null && !string.IsNullOrEmpty(trangThai.UrlAnh))
                    {
                        var imagePath = Path.Combine("wwwroot", trangThai.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    await _trangthaiRepository.DeleteAsync(id);
                }

                foreach (var id in trangThaiToHide)
                {
                    var trangThai = await _trangthaiRepository.GetByIdAsync(id);
                    if (trangThai != null)
                    {
                        trangThai.TTHienThi = false;
                        await _trangthaiRepository.UpdateAsync(trangThai);
                    }
                }

                if (trangThaiToDelete.Count > 0)
                    TempData["Message"] = $"Đã xóa {trangThaiToDelete.Count} trạng thái không có hóa đơn.";
                if (trangThaiToHide.Count > 0)
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã ẩn {trangThaiToHide.Count} trạng thái có hóa đơn.";
            }
            else
            {
                foreach (var id in selectedTrangThaiMaTT)
                {
                    var trangThai = await _trangthaiRepository.GetByIdAsync(id);
                    if (trangThai != null)
                    {
                        trangThai.TTHienThi = false;
                        await _trangthaiRepository.UpdateAsync(trangThai);
                    }
                }
                TempData["Message"] = $"Đã ẩn {selectedTrangThaiMaTT.Count} trạng thái.";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int maTT, bool status)
        {
            var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
            if (trangThai == null)
                return Json(new { success = false, message = "Trạng thái không tồn tại." });

            trangThai.TTHienThi = status;
            await _trangthaiRepository.UpdateAsync(trangThai);

            return Json(new { success = true });
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Display(int maTT)
        {
            var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
            if (trangThai == null)
                return NotFound();
            return View(trangThai);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TrangThai trangThai, IFormFile urlAnh)
        {
            ModelState.Remove("UrlAnh"); // Loại bỏ validation cho UrlAnh

            if (urlAnh == null || urlAnh.Length == 0)
            {
                ModelState.AddModelError("UrlAnh", "Vui lòng chọn ảnh trạng thái.");
            }

            if (ModelState.IsValid)
            {
                if (urlAnh != null && urlAnh.Length > 0)
                {
                    trangThai.UrlAnh = await SaveImage(urlAnh);
                }

                trangThai.TTHienThi = true;
                await _trangthaiRepository.AddAsync(trangThai);
                return RedirectToAction(nameof(Index));
            }

            return View(trangThai);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int maTT)
        {
            var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
            if (trangThai == null)
                return NotFound();
            return View(trangThai);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int maTT, TrangThai trangThai, IFormFile urlAnh)
        {
            ModelState.Remove("UrlAnh");
            if (maTT != trangThai.MaTT)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingTrangThai = await _trangthaiRepository.GetByIdAsync(maTT);
                if (existingTrangThai == null)
                    return NotFound();

                if (urlAnh != null && urlAnh.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingTrangThai.UrlAnh))
                    {
                        var oldPath = Path.Combine("wwwroot", existingTrangThai.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    existingTrangThai.UrlAnh = await SaveImage(urlAnh);
                }

                existingTrangThai.TenTT = trangThai.TenTT;
                existingTrangThai.TTHienThi = trangThai.TTHienThi;

                await _trangthaiRepository.UpdateAsync(existingTrangThai);
                return RedirectToAction(nameof(Index));
            }

            return View(trangThai);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int maTT)
        {
            var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
            if (trangThai == null)
                return NotFound();
            return View(trangThai);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int maTT)
        {
            var trangThai = await _trangthaiRepository.GetByIdAsync(maTT);
            if (trangThai != null)
            {
                if (!string.IsNullOrEmpty(trangThai.UrlAnh))
                {
                    var imagePath = Path.Combine("wwwroot", trangThai.UrlAnh.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                await _trangthaiRepository.DeleteAsync(maTT);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XemChiTietHoaDonTheoTrangThai(List<int> selectedTrangThaiMaTT)
        {
            if (selectedTrangThaiMaTT == null || selectedTrangThaiMaTT.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var allHoaDons = (await _lichSuTTHDRepository.GetAllAsync()).ToArray();
            string tuKhoaTimKiem = "";
            bool dauTien = true;

            for (int i = 0; i < allHoaDons.Length; i++)
            {
                for (int j = 0; j < selectedTrangThaiMaTT.Count; j++)
                {
                    if (allHoaDons[i].MaTT == selectedTrangThaiMaTT[j])
                    {
                        if (!dauTien)
                        {
                            tuKhoaTimKiem += ",";
                        }
                        tuKhoaTimKiem += allHoaDons[i].SoHD.ToString();
                        dauTien = false;
                        break;
                    }
                }
            }

            if (tuKhoaTimKiem == "")
            {
                return RedirectToAction("DanhSachHoaDon", "QuanLyHoaDon", new { tuKhoaTimKiem = "", chonTrangThai = "tatCa", chonPTTT = (int?)null, tuNgay = (DateTime?)null, denNgay = (DateTime?)null, sapXepGia = "macDinh" });
            }

            return RedirectToAction("DanhSachHoaDon", "QuanLyHoaDon", new
            {
                tuKhoaTimKiem = tuKhoaTimKiem + ",",
                chonTrangThai = "tatCa",
                chonPTTT = (int?)null,
                tuNgay = (DateTime?)null,
                denNgay = (DateTime?)null,
                sapXepGia = "macDinh"
            });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine("wwwroot/images/TrangThai", fileName);
            var directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/TrangThai/" + fileName;
        }
    }
}