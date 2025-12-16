using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhMucController : Controller
    {
        private readonly ISanPhamRepository _sanphamRepository;
        private readonly IDanhMucRepository _danhmucRepository;

        public DanhMucController(ISanPhamRepository productRepository, IDanhMucRepository danhmucRepository)
        {
            _sanphamRepository = productRepository;
            _danhmucRepository = danhmucRepository;
        }

        // Hiển thị danh sách danh mục và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi", string chonDeXuat = "tatCaDeXuat")
        {
            var danhmucs = (await _danhmucRepository.GetAllAsync()).ToList();
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;
            ViewData["chonDeXuat"] = chonDeXuat;

            // Lọc theo trạng thái hiển thị
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    danhmucs = danhmucs.Where(c => !c.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    danhmucs = danhmucs.Where(c => c.TTHienThi).ToList();
                    break;
            }

            // Lọc theo trạng thái đề xuất
            switch (chonDeXuat)
            {
                case "tatCaDeXuat":
                    break;
                case "tatDeXuat":
                    danhmucs = danhmucs.Where(c => !c.TTDeXuat).ToList();
                    break;
                case "batDeXuat":
                    danhmucs = danhmucs.Where(c => c.TTDeXuat).ToList();
                    break;
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ketQuaTimKiem = new List<DanhMuc>();
                var diemSo = new List<int>();

                foreach (var danhmuc in danhmucs)
                {
                    int diem = TinhSoTuKhop(danhmuc, tuKhoaArray);
                    if (diem > 0)
                    {
                        ketQuaTimKiem.Add(danhmuc);
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
                            var tempDanhMuc = ketQuaTimKiem[i];
                            ketQuaTimKiem[i] = ketQuaTimKiem[j];
                            ketQuaTimKiem[j] = tempDanhMuc;
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
                    ViewBag.KetQuaTimKiem = "Không tìm thấy danh mục liên quan.";
                    return View(danhmucs.Take(3).ToList());
                }
            }

            return View(danhmucs);
        }

        // Tính số từ khớp cho tìm kiếm
        private int TinhSoTuKhop(DanhMuc danhmuc, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var tenDanhMuc = (" " + (danhmuc.TenDM?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (tenDanhMuc.Contains(tuKhoa))
                    soTuKhop += 2;
            }
            return soTuKhop;
        }

        // Gợi ý tìm kiếm
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "", string chonDeXuat = "")
        {
            var danhmucs = (await _danhmucRepository.GetAllAsync()).ToList();

            // Lọc theo trạng thái hiển thị
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    danhmucs = danhmucs.Where(c => !c.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    danhmucs = danhmucs.Where(c => c.TTHienThi).ToList();
                    break;
            }

            // Lọc theo trạng thái đề xuất
            switch (chonDeXuat)
            {
                case "tatCaDeXuat":
                    break;
                case "tatDeXuat":
                    danhmucs = danhmucs.Where(c => !c.TTDeXuat).ToList();
                    break;
                case "batDeXuat":
                    danhmucs = danhmucs.Where(c => c.TTDeXuat).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            foreach (var danhmuc in danhmucs)
            {
                int diem = TinhSoKyTuKhop(danhmuc, tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new { maDM = danhmuc.MaDM, tenDM = danhmuc.TenDM });
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
        private int TinhSoKyTuKhop(DanhMuc danhmuc, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var tenDanhMuc = danhmuc.TenDM?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (tenDanhMuc.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
            }
            return soKyTuKhop;
        }

        // Kiểm tra danh mục có sản phẩm liên quan không
        [HttpPost]
        public async Task<IActionResult> CheckDanhMucInSanPham(List<int> selectedDanhMucMaDM)
        {
            var allSanPhams = await _sanphamRepository.GetAllAsync();
            var result = new List<object>();

            foreach (var maDM in selectedDanhMucMaDM)
            {
                var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
                bool hasSanPham = allSanPhams.Any(p => p.MaDM == maDM);
                result.Add(new { maDM = danhmuc.MaDM, tenDM = danhmuc.TenDM, hasSanPham });
            }

            return Json(result);
        }

        // Xử lý xóa hoặc ẩn nhiều danh mục
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedDanhMucMaDM, bool isDelete = false)
        {
            if (selectedDanhMucMaDM == null || selectedDanhMucMaDM.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một danh mục để xử lý.";
                return RedirectToAction("Index");
            }

            var allSanPhams = await _sanphamRepository.GetAllAsync();
            var danhmucToDelete = new List<int>();
            var danhmucToHide = new List<int>();

            foreach (var maDM in selectedDanhMucMaDM)
            {
                bool hasSanPham = allSanPhams.Any(p => p.MaDM == maDM);
                if (hasSanPham)
                    danhmucToHide.Add(maDM);
                else
                    danhmucToDelete.Add(maDM);
            }

            if (isDelete)
            {
                foreach (var id in danhmucToDelete)
                    await _danhmucRepository.DeleteAsync(id);

                foreach (var id in danhmucToHide)
                {
                    var danhmuc = await _danhmucRepository.GetByIdAsync(id);
                    if (danhmuc != null)
                    {
                        danhmuc.TTHienThi = false;
                        await _danhmucRepository.UpdateAsync(danhmuc);
                    }
                }

                if (danhmucToDelete.Count > 0)
                    TempData["Message"] = $"Đã xóa {danhmucToDelete.Count} danh mục không có sản phẩm.";
                if (danhmucToHide.Count > 0)
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã ẩn {danhmucToHide.Count} danh mục có sản phẩm.";
            }
            else
            {
                foreach (var id in selectedDanhMucMaDM)
                {
                    var danhmuc = await _danhmucRepository.GetByIdAsync(id);
                    if (danhmuc != null)
                    {
                        danhmuc.TTHienThi = false;
                        await _danhmucRepository.UpdateAsync(danhmuc);
                    }
                }
                TempData["Message"] = $"Đã ẩn {selectedDanhMucMaDM.Count} danh mục.";
            }

            return RedirectToAction("Index");
        }

        // Thay đổi trạng thái hiển thị
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int maDM, bool status)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc == null)
                return Json(new { success = false, message = "Danh mục không tồn tại." });

            danhmuc.TTHienThi = status;
            await _danhmucRepository.UpdateAsync(danhmuc);

            return Json(new { success = true });
        }

        // Thay đổi trạng thái đề xuất
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDeXuat(int maDM, bool status)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc == null)
                return Json(new { success = false, message = "Danh mục không tồn tại." });

            danhmuc.TTDeXuat = status;
            await _danhmucRepository.UpdateAsync(danhmuc);

            return Json(new { success = true });
        }

        // Cập nhật trạng thái hiển thị hàng loạt
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(string selectedDanhMucMaDM, bool newTrangThai)
        {
            if (string.IsNullOrEmpty(selectedDanhMucMaDM))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một danh mục để cập nhật." });
            }

            var danhSachMaDM = new List<int>();
            var mangId = selectedDanhMucMaDM.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int maDM))
                {
                    danhSachMaDM.Add(maDM);
                }
            }

            if (danhSachMaDM.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách danh mục không hợp lệ." });
            }

            var updatedDanhMuc = new List<object>();
            for (int i = 0; i < danhSachMaDM.Count; i++)
            {
                var danhMuc = await _danhmucRepository.GetByIdAsync(danhSachMaDM[i]);
                if (danhMuc != null)
                {
                    var trangThaiCu = danhMuc.TTHienThi;
                    danhMuc.TTHienThi = newTrangThai;
                    await _danhmucRepository.UpdateAsync(danhMuc);
                    updatedDanhMuc.Add(new
                    {
                        maDM = danhMuc.MaDM,
                        tenDM = danhMuc.TenDM,
                        trangThaiCu = trangThaiCu ? "Bật" : "Tắt",
                        trangThaiMoi = newTrangThai ? "Bật" : "Tắt"
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái hiển thị cho {danhSachMaDM.Count} danh mục.",
                updatedDanhMuc = updatedDanhMuc
            });
        }

        // Cập nhật trạng thái đề xuất hàng loạt
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiDeXuat(string selectedDanhMucMaDM, bool newTrangThaiDeXuat)
        {
            if (string.IsNullOrEmpty(selectedDanhMucMaDM))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một danh mục để cập nhật." });
            }

            var danhSachMaDM = new List<int>();
            var mangId = selectedDanhMucMaDM.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int maDM))
                {
                    danhSachMaDM.Add(maDM);
                }
            }

            if (danhSachMaDM.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách danh mục không hợp lệ." });
            }

            var updatedDanhMuc = new List<object>();
            for (int i = 0; i < danhSachMaDM.Count; i++)
            {
                var danhMuc = await _danhmucRepository.GetByIdAsync(danhSachMaDM[i]);
                if (danhMuc != null)
                {
                    var trangThaiDeXuatCu = danhMuc.TTDeXuat;
                    danhMuc.TTDeXuat = newTrangThaiDeXuat;
                    await _danhmucRepository.UpdateAsync(danhMuc);
                    updatedDanhMuc.Add(new
                    {
                        maDM = danhMuc.MaDM,
                        tenDM = danhMuc.TenDM,
                        trangThaiDeXuatCu = trangThaiDeXuatCu ? "Bật" : "Tắt",
                        trangThaiDeXuatMoi = newTrangThaiDeXuat ? "Bật" : "Tắt"
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái đề xuất cho {danhSachMaDM.Count} danh mục.",
                updatedDanhMuc = updatedDanhMuc
            });
        }

        // Hiển thị chi tiết danh mục
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Display(int maDM)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc == null)
                return NotFound();
            return View(danhmuc);
        }

        // Hiển thị form thêm danh mục
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm danh mục
        [HttpPost]
        public async Task<IActionResult> Add(DanhMuc danhmuc)
        {
            if (ModelState.IsValid)
            {
                await _danhmucRepository.AddAsync(danhmuc);
                return RedirectToAction(nameof(Index));
            }
            return View(danhmuc);
        }

        // Hiển thị form sửa danh mục
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int maDM)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc == null)
                return NotFound();
            return View(danhmuc);
        }

        // Xử lý sửa danh mục
        [HttpPost]
        public async Task<IActionResult> Update(int maDM, DanhMuc danhmuc)
        {
            if (maDM != danhmuc.MaDM)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _danhmucRepository.UpdateAsync(danhmuc);
                return RedirectToAction(nameof(Index));
            }
            return View(danhmuc);
        }

        // Hiển thị form xác nhận xóa danh mục
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int maDM)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc == null)
                return NotFound();
            return View(danhmuc);
        }

        // Xử lý xóa danh mục
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int maDM)
        {
            var danhmuc = await _danhmucRepository.GetByIdAsync(maDM);
            if (danhmuc != null)
                await _danhmucRepository.DeleteAsync(maDM);
            return RedirectToAction(nameof(Index));
        }

        // Xem chi tiết sản phẩm theo danh mục
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XemChiTietSanPhamTheoDanhMuc(List<int> selectedDanhMucMaDM)
        {
            if (selectedDanhMucMaDM == null || selectedDanhMucMaDM.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var allSanpham = (await _sanphamRepository.GetAllAsync()).ToArray();
            string tuKhoaTimKiem = "";
            bool dauTien = true;

            for (int i = 0; i < allSanpham.Length; i++)
            {
                for (int j = 0; j < selectedDanhMucMaDM.Count; j++)
                {
                    if (allSanpham[i].MaDM == selectedDanhMucMaDM[j])
                    {
                        if (!dauTien)
                        {
                            tuKhoaTimKiem = tuKhoaTimKiem + ",";
                        }
                        tuKhoaTimKiem = tuKhoaTimKiem + allSanpham[i].MaSP.ToString();
                        dauTien = false;
                        break;
                    }
                }
            }

            if (tuKhoaTimKiem == "")
            {
                return RedirectToAction("Index", "SanPham", new { tuKhoaTimKiem = "", chonTrangThai = "tatCa", chonDanhMuc = "" });
            }

            return RedirectToAction("Index", "SanPham", new
            {
                tuKhoaTimKiem = tuKhoaTimKiem + ",",
                chonTrangThai = "tatCa",
                chonDanhMuc = ""
            });
        }
    }
}