using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
   [Area("Admin")]
    public class PTTTController : Controller
    {
        private readonly IHoaDonRepository _hoadonRepository;
        private readonly IPTTTRepository _ptttRepository;

        public PTTTController(IHoaDonRepository hoadonRepository, IPTTTRepository ptttRepository)
        {
            _hoadonRepository = hoadonRepository;
            _ptttRepository = ptttRepository;
        }

        // Hiển thị danh sách phương thức thanh toán và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi")
        {
            var pttts = (await _ptttRepository.GetAllAsync()).ToList();
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;

            // Lọc theo trạng thái
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    pttts = pttts.Where(p => !p.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    pttts = pttts.Where(p => p.TTHienThi).ToList();
                    break;
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ketQuaTimKiem = new List<PTTT>();
                var diemSo = new List<int>();

                foreach (var pttt in pttts)
                {
                    int diem = TinhSoTuKhop(pttt, tuKhoaArray);
                    if (diem > 0)
                    {
                        ketQuaTimKiem.Add(pttt);
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
                            var tempPTTT = ketQuaTimKiem[i];
                            ketQuaTimKiem[i] = ketQuaTimKiem[j];
                            ketQuaTimKiem[j] = tempPTTT;
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
                    ViewBag.KetQuaTimKiem = "Không tìm thấy phương thức thanh toán liên quan.";
                    return View(pttts.Take(3).ToList());
                }
            }

            return View(pttts);
        }

        // Tính số từ khớp cho tìm kiếm
        private int TinhSoTuKhop(PTTT pttt, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var tenPT = (" " + (pttt.TenPT?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (tenPT.Contains(tuKhoa))
                    soTuKhop += 2;
            }
            return soTuKhop;
        }

        // Gợi ý tìm kiếm
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var pttts = (await _ptttRepository.GetAllAsync()).ToList();

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    pttts = pttts.Where(p => !p.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    pttts = pttts.Where(p => p.TTHienThi).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            foreach (var pttt in pttts)
            {
                int diem = TinhSoKyTuKhop(pttt, tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new { maPT = pttt.MaPT, tenPT = pttt.TenPT });
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
        private int TinhSoKyTuKhop(PTTT pttt, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var tenPT = pttt.TenPT?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (tenPT.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
            }
            return soKyTuKhop;
        }

        // Kiểm tra phương thức thanh toán có hóa đơn liên quan không
        [HttpPost]
        public async Task<IActionResult> CheckPTTTInHoaDon(List<int> selectedPTTTMaPT)
        {
            var allHoaDons = await _hoadonRepository.GetAllAsync();
            var result = new List<object>();

            foreach (var maPT in selectedPTTTMaPT)
            {
                var pttt = await _ptttRepository.GetByIdAsync(maPT);
                bool hasHoaDon = allHoaDons.Any(h => h.MaPT == maPT);
                result.Add(new { maPT = pttt.MaPT, tenPT = pttt.TenPT, hasHoaDon });
            }

            return Json(result);
        }

        // Xử lý xóa hoặc ẩn nhiều phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedPTTTMaPT, bool isDelete = false)
        {
            if (selectedPTTTMaPT == null || selectedPTTTMaPT.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một phương thức thanh toán để xử lý.";
                return RedirectToAction("Index");
            }

            var allHoaDons = await _hoadonRepository.GetAllAsync();
            var ptttToDelete = new List<int>();
            var ptttToHide = new List<int>();

            foreach (var maPT in selectedPTTTMaPT)
            {
                bool hasHoaDon = allHoaDons.Any(h => h.MaPT == maPT);
                if (hasHoaDon)
                    ptttToHide.Add(maPT);
                else
                    ptttToDelete.Add(maPT);
            }

            if (isDelete)
            {
                foreach (var id in ptttToDelete)
                    await _ptttRepository.DeleteAsync(id);

                foreach (var id in ptttToHide)
                {
                    var pttt = await _ptttRepository.GetByIdAsync(id);
                    if (pttt != null)
                    {
                        pttt.TTHienThi = false;
                        await _ptttRepository.UpdateAsync(pttt);
                    }
                }

                if (ptttToDelete.Count > 0)
                    TempData["Message"] = $"Đã xóa {ptttToDelete.Count} phương thức thanh toán không có hóa đơn.";
                if (ptttToHide.Count > 0)
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã ẩn {ptttToHide.Count} phương thức thanh toán có hóa đơn.";
            }
            else
            {
                foreach (var id in selectedPTTTMaPT)
                {
                    var pttt = await _ptttRepository.GetByIdAsync(id);
                    if (pttt != null)
                    {
                        pttt.TTHienThi = false;
                        await _ptttRepository.UpdateAsync(pttt);
                    }
                }
                TempData["Message"] = $"Đã ẩn {selectedPTTTMaPT.Count} phương thức thanh toán.";
            }

            return RedirectToAction("Index");
        }

        // Thay đổi trạng thái hiển thị
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int maPT, bool status)
        {
            var pttt = await _ptttRepository.GetByIdAsync(maPT);
            if (pttt == null)
                return Json(new { success = false, message = "Phương thức thanh toán không tồn tại." });

            pttt.TTHienThi = status;
            await _ptttRepository.UpdateAsync(pttt);

            return Json(new { success = true });
        }

        // Hiển thị chi tiết phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Display(int maPT)
        {
            var pttt = await _ptttRepository.GetByIdAsync(maPT);
            if (pttt == null)
                return NotFound();
            return View(pttt);
        }

        // Hiển thị form thêm phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm phương thức thanh toán
        [HttpPost]
        public async Task<IActionResult> Add(PTTT pttt)
        {
            if (ModelState.IsValid)
            {
                await _ptttRepository.AddAsync(pttt);
                return RedirectToAction(nameof(Index));
            }
            return View(pttt);
        }

        // Hiển thị form sửa phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int maPT)
        {
            var pttt = await _ptttRepository.GetByIdAsync(maPT);
            if (pttt == null)
                return NotFound();
            return View(pttt);
        }

        // Xử lý sửa phương thức thanh toán
        [HttpPost]
        public async Task<IActionResult> Update(int maPT, PTTT pttt)
        {
            if (maPT != pttt.MaPT)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _ptttRepository.UpdateAsync(pttt);
                return RedirectToAction(nameof(Index));
            }
            return View(pttt);
        }

        // Hiển thị form xác nhận xóa phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int maPT)
        {
            var pttt = await _ptttRepository.GetByIdAsync(maPT);
            if (pttt == null)
                return NotFound();
            return View(pttt);
        }

        // Xử lý xóa phương thức thanh toán
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int maPT)
        {
            var pttt = await _ptttRepository.GetByIdAsync(maPT);
            if (pttt != null)
                await _ptttRepository.DeleteAsync(maPT);
            return RedirectToAction(nameof(Index));
        }

        // Xem chi tiết hóa đơn theo phương thức thanh toán
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XemChiTietHoaDonTheoPTTT(List<int> selectedPTTTMaPT)
        {
            if (selectedPTTTMaPT == null || selectedPTTTMaPT.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var allHoaDons = (await _hoadonRepository.GetAllAsync()).ToArray();
            string tuKhoaTimKiem = "";
            bool dauTien = true;

            for (int i = 0; i < allHoaDons.Length; i++)
            {
                for (int j = 0; j < selectedPTTTMaPT.Count; j++)
                {
                    if (allHoaDons[i].MaPT == selectedPTTTMaPT[j])
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
    }
}