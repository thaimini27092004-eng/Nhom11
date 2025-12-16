using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;
using WebsiteBanHang.Models.QuanLyHoaDon.ViewModel;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Models.QuanLyTrangThai;
using WebsiteBanHang.Services.GuiEmailTHongBao;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLyHoaDonController : Controller
    {
        private readonly IQuanLyHoaDonRepository _hoaDonRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IPTTTRepository _ptttRepository;
        private readonly ITrangThaiRepository _trangThaiRepository;
        private readonly ICTHDRepository _cthdRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;
        private readonly IEmailService _emailService;
        public QuanLyHoaDonController(
            IQuanLyHoaDonRepository hoaDonRepository,
            IKhachHangRepository khachHangRepository,
            IPTTTRepository ptttRepository,
            ITrangThaiRepository trangThaiRepository,
            ICTHDRepository cthdRepository,
            ApplicationDbContext context,
            ILichSuTTHDRepository lichSuTTHDRepository,
            IEmailService emailService)
        {
            _hoaDonRepository = hoaDonRepository;
            _khachHangRepository = khachHangRepository;
            _ptttRepository = ptttRepository;
            _trangThaiRepository = trangThaiRepository;
            _cthdRepository = cthdRepository;
            _context = context;
            _lichSuTTHDRepository = lichSuTTHDRepository;
            _emailService = emailService;
        }

        private async Task ganDuLieuVaoViewBag()
        {
            var pttts = (await _ptttRepository.GetAllAsync()).Where(p => p.TTHienThi).ToList();
            var trangThais = (await _trangThaiRepository.GetAllAsync()).Where(t => t.TTHienThi).ToList();
            ViewBag.PTTTs = pttts;
            ViewBag.TrangThais = trangThais;
        }

        // Hiển thị danh sách hóa đơn
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> DanhSachHoaDon(string tuKhoaTimKiem = "", string chonTrangThai = "tatCa", int? chonPTTT = null, DateTime? tuNgay = null, DateTime? denNgay = null, string sapXepGia = "macDinh")
        {
            var hoaDons = (await _hoaDonRepository.GetAllAsync()).ToList();
            await ganDuLieuVaoViewBag();
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;
            ViewData["chonPTTT"] = chonPTTT;
            ViewData["tuNgay"] = tuNgay?.ToString("yyyy-MM-dd");
            ViewData["denNgay"] = denNgay?.ToString("yyyy-MM-dd");
            ViewData["sapXepGia"] = sapXepGia;

            // Lọc theo trạng thái
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                default:
                    if (int.TryParse(chonTrangThai, out int maTT))
                    {
                        var soHDs = (await _lichSuTTHDRepository.GetAllAsync())
                            .Where(ls => ls.MaTT == maTT)
                            .Select(ls => ls.SoHD)
                            .Distinct()
                            .ToList();
                        hoaDons = hoaDons.Where(h => soHDs.Contains(h.SoHD)).ToList();
                    }
                    break;
            }

            // Lọc theo PTTT
            if (chonPTTT.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.MaPT == chonPTTT.Value).ToList();
            }

            // Lọc theo thời gian
            if (tuNgay.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayDat >= tuNgay.Value).ToList();
            }
            if (denNgay.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayDat <= denNgay.Value.AddDays(1).AddTicks(-1)).ToList();
            }

            // Sắp xếp theo giá
            switch (sapXepGia)
            {
                case "thapDenCao":
                    hoaDons = hoaDons.OrderBy(h => h.CTHDs.Sum(ct => ct.SL * ct.DonGia)).ToList();
                    break;
                case "caoDenThap":
                    hoaDons = hoaDons.OrderByDescending(h => h.CTHDs.Sum(ct => ct.SL * ct.DonGia)).ToList();
                    break;
                default:
                    break;
            }

            // Chuyển đổi sang ViewModel
            var hoaDonViewModels = new List<HoaDonViewModel>();
            for (int i = 0; i < hoaDons.Count; i++)
            {
                var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDons[i].SoHD);
                var viewModel = new HoaDonViewModel
                {
                    SoHD = hoaDons[i].SoHD,
                    NgayDat = hoaDons[i].NgayDat,
                    TenKH = hoaDons[i].KhachHang?.TenKH ?? "Không xác định",
                    TongTien = hoaDons[i].CTHDs?.Sum(ct => ct.SL * ct.DonGia) ?? 0,
                    TenPT = hoaDons[i].PTTT?.TenPT ?? "Không xác định",
                    TenTT = latestStatus.TrangThai?.TenTT ?? "Không xác định",
                    MaKhoList = hoaDons[i].CTHDs?.Select(ct => ct.MaKho).Distinct().ToList() ?? new List<int>()
                };
                hoaDonViewModels.Add(viewModel);
            }

            // Tìm kiếm
            if (string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                return View(hoaDonViewModels);
            }

            if (tuKhoaTimKiem.Contains(","))
            {
                var idArray = tuKhoaTimKiem.Split(',');
                var danhSachIdHopLe = new List<int>();

                for (int i = 0; i < idArray.Length; i++)
                {
                    var id = idArray[i].Trim();
                    if (int.TryParse(id, out int soHD))
                    {
                        danhSachIdHopLe.Add(soHD);
                    }
                }

                var ketQuaTimKiem = hoaDonViewModels.Where(h => danhSachIdHopLe.Contains(h.SoHD)).ToList();

                if (ketQuaTimKiem.Count > 0)
                {
                    return View(ketQuaTimKiem);
                }
                else
                {
                    ViewBag.KetQuaTimKiem = "Không tìm thấy hóa đơn với mã đã nhập.";
                    var goiY = hoaDonViewModels.Take(3).ToList();
                    return View(goiY);
                }
            }

            var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var ketQuaTimKiem2 = new List<HoaDonViewModel>();
            var diemSo = new List<int>();

            for (int i = 0; i < hoaDonViewModels.Count; i++)
            {
                int diem = TinhSoTuKhop(hoaDons[i], tuKhoaArray); // Sử dụng hoaDons để tính điểm
                if (diem > 0)
                {
                    ketQuaTimKiem2.Add(hoaDonViewModels[i]);
                    diemSo.Add(diem);
                }
            }

            for (int i = 0; i < ketQuaTimKiem2.Count - 1; i++)
            {
                for (int j = i + 1; j < ketQuaTimKiem2.Count; j++)
                {
                    if (diemSo[i] < diemSo[j])
                    {
                        var temp = ketQuaTimKiem2[i];
                        ketQuaTimKiem2[i] = ketQuaTimKiem2[j];
                        ketQuaTimKiem2[j] = temp;
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            if (ketQuaTimKiem2.Count > 0)
            {
                return View(ketQuaTimKiem2);
            }
            else
            {
                ViewBag.KetQuaTimKiem = "Không có hóa đơn liên quan.";
                var goiY = hoaDonViewModels.Take(3).ToList();
                return View(goiY);
            }
        }

        // Gợi ý tìm kiếm
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var hoaDons = (await _hoaDonRepository.GetAllAsync()).ToList();

            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                default:
                    if (int.TryParse(chonTrangThai, out int maTT))
                    {
                        var soHDs = (await _lichSuTTHDRepository.GetAllAsync())
                           .Where(ls => ls.MaTT == maTT)
                           .Select(ls => ls.SoHD)
                           .Distinct()
                           .ToList();
                        hoaDons = hoaDons.Where(h => soHDs.Contains(h.SoHD)).ToList();
                    }
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            if (tuKhoa.Contains(","))
            {
                var idArray = tuKhoa.Split(',');
                var danhSachIdHopLe = new List<int>();

                for (int i = 0; i < idArray.Length; i++)
                {
                    var id = idArray[i].Trim();
                    if (int.TryParse(id, out int soHD))
                    {
                        danhSachIdHopLe.Add(soHD);
                    }
                }

                int dem = 0;
                for (int i = 0; i < hoaDons.Count && dem < 3; i++)
                {
                    for (int j = 0; j < danhSachIdHopLe.Count; j++)
                    {
                        if (hoaDons[i].SoHD == danhSachIdHopLe[j])
                        {
                            goiY.Add(new
                            {
                                soHD = hoaDons[i].SoHD,
                                tenKH = hoaDons[i].KhachHang?.TenKH,
                                emailKH = hoaDons[i].KhachHang?.EmailKH,
                                maKH = hoaDons[i].MaKH,
                                ngayDat = hoaDons[i].NgayDat.ToString("dd/MM/yyyy")
                            });
                            dem++;
                            break;
                        }
                    }
                }

                return Json(goiY);
            }

            for (int i = 0; i < hoaDons.Count; i++)
            {
                int diem = TinhSoKyTuKhop(hoaDons[i], tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new
                    {
                        soHD = hoaDons[i].SoHD,
                        tenKH = hoaDons[i].KhachHang?.TenKH,
                        emailKH = hoaDons[i].KhachHang?.EmailKH,
                        maKH = hoaDons[i].MaKH,
                        ngayDat = hoaDons[i].NgayDat.ToString("dd/MM/yyyy")
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

        private int TinhSoTuKhop(HoaDon hoaDon, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var tenKH = (" " + (hoaDon.KhachHang?.TenKH?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var emailKH = (" " + (hoaDon.KhachHang?.EmailKH?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var maKH = hoaDon.MaKH.ToString();

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (tenKH.Contains(tuKhoa))
                    soTuKhop += 2;
                else if (emailKH.Contains(tuKhoa))
                    soTuKhop += 1;
                else if (maKH.Contains(tuKhoa))
                    soTuKhop += 2;
            }
            return soTuKhop;
        }

        private int TinhSoKyTuKhop(HoaDon hoaDon, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var tenKH = hoaDon.KhachHang?.TenKH?.ToLower() ?? "";
            var emailKH = hoaDon.KhachHang?.EmailKH?.ToLower() ?? "";
            var maKH = hoaDon.MaKH.ToString();
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (tenKH.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
                else if (emailKH.Contains(tu))
                    soKyTuKhop += tu.Length;
                else if (maKH.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
            }
            return soKyTuKhop;
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedSoHD, bool isDelete = false)
        {
            if (selectedSoHD == null || selectedSoHD.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một hóa đơn để xử lý.";
                return RedirectToAction("DanhSachHoaDon");
            }

            var hoaDonToDelete = new List<int>();
            var trangThaiHuy = 9; // Giả sử MaTT = 9 là trạng thái "Hủy"

            if (isDelete)
            {
                for (int i = 0; i < selectedSoHD.Count; i++)
                {
                    var hoaDon = await _hoaDonRepository.GetByIdAsync(selectedSoHD[i]);
                    if (hoaDon != null)
                    {
                        var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDon.SoHD);
                        if (latestStatus.MaTT == trangThaiHuy)
                        {
                            hoaDonToDelete.Add(selectedSoHD[i]);
                        }
                    }
                }

                for (int i = 0; i < hoaDonToDelete.Count; i++)
                {
                    await _hoaDonRepository.DeleteAsync(hoaDonToDelete[i]);
                }

                if (hoaDonToDelete.Count > 0)
                {
                    TempData["Message"] = $"Đã xóa {hoaDonToDelete.Count} hóa đơn ở trạng thái Hủy.";
                }
                else
                {
                    TempData["Error"] = "Không có hóa đơn nào ở trạng thái Hủy để xóa.";
                }
            }
            else
            {
                TempData["Error"] = "Vui lòng chọn 'Xóa' để thực hiện hành động.";
            }

            return RedirectToAction("DanhSachHoaDon");
        }

        [HttpPost]
        public async Task<IActionResult> CheckHoaDonStatus(List<int> selectedSoHD)
        {
            var result = new List<object>();
            var trangThaiHuy = 9; // Giả sử MaTT = 4 là trạng thái "Hủy"

            for (int i = 0; i < selectedSoHD.Count; i++)
            {
                var hoaDon = await _hoaDonRepository.GetByIdAsync(selectedSoHD[i]);
                if (hoaDon != null)
                {
                    var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDon.SoHD);
                    bool canDelete = latestStatus.MaTT == trangThaiHuy;
                    result.Add(new
                    {
                        soHD = hoaDon.SoHD,
                        tenKH = hoaDon.KhachHang?.TenKH,
                        canDelete
                    });
                }
            }

            return Json(result);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(string selectedSoHD, int newMaTT)
        {
            if (string.IsNullOrEmpty(selectedSoHD))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một hóa đơn để cập nhật." });
            }

            var danhSachSoHD = new List<int>();
            var mangId = selectedSoHD.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int soHD))
                {
                    danhSachSoHD.Add(soHD);
                }
            }

            if (danhSachSoHD.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách hóa đơn không hợp lệ." });
            }

            var trangThaiMoi = await _trangThaiRepository.GetByIdAsync(newMaTT);
            if (trangThaiMoi == null || !trangThaiMoi.TTHienThi)
            {
                return Json(new { success = false, message = "Trạng thái được chọn không hợp lệ hoặc không hiển thị." });
            }

            var updateHoaDon = new List<object>();
            for (int i = 0; i < danhSachSoHD.Count; i++)
            {
                var hoaDon = await _hoaDonRepository.GetByIdAsync(danhSachSoHD[i]);
                if (hoaDon != null)
                {
                    var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDon.SoHD);
                    var lichSu = new LichSuTTHD
                    {
                        SoHD = hoaDon.SoHD,
                        MaTT = newMaTT,
                        ThoiGianThayDoi = DateTime.Now,
                        GhiChu = "Cập nhật trạng thái qua Admin"
                    };
                    await _lichSuTTHDRepository.AddAsync(lichSu);


                    // Gửi email thông báo cập nhật trạng thái
                    var khachHang = await _khachHangRepository.GetByIdAsync(hoaDon.MaKH);
                    if (khachHang != null && !string.IsNullOrEmpty(khachHang.EmailKH))
                    {
                        await _emailService.GuiEmailCapNhatTrangThaiAsync(hoaDon.SoHD, khachHang.EmailKH, trangThaiMoi.TenTT);
                    }

                    updateHoaDon.Add(new
                    {
                        soHD = hoaDon.SoHD,
                        tenKH = hoaDon.KhachHang?.TenKH,
                        trangThaiCu = latestStatus.TrangThai?.TenTT ?? "Không có",
                        trangThaiMoi = trangThaiMoi.TenTT
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái cho {danhSachSoHD.Count} hóa đơn.",
                updatedHoaDon = updateHoaDon
            });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatPhuongThucThanhToan(string selectedSoHD, int newMaPT)
        {
            if (string.IsNullOrEmpty(selectedSoHD))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một hóa đơn để cập nhật." });
            }

            var danhSachSoHD = new List<int>();
            var mangId = selectedSoHD.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int soHD))
                {
                    danhSachSoHD.Add(soHD);
                }
            }

            if (danhSachSoHD.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách hóa đơn không hợp lệ." });
            }

            var ptttMoi = await _ptttRepository.GetByIdAsync(newMaPT);
            if (ptttMoi == null || !ptttMoi.TTHienThi)
            {
                return Json(new { success = false, message = "Phương thức thanh toán không hợp lệ hoặc không hiển thị." });
            }

            var updateHoaDon = new List<object>();
            for (int i = 0; i < danhSachSoHD.Count; i++)
            {
                var hoaDon = await _hoaDonRepository.GetByIdAsync(danhSachSoHD[i]);
                if (hoaDon != null)
                {
                    var ptttCu = await _ptttRepository.GetByIdAsync(hoaDon.MaPT);
                    hoaDon.MaPT = newMaPT;
                    await _hoaDonRepository.UpdateAsync(hoaDon);
                    updateHoaDon.Add(new
                    {
                        soHD = hoaDon.SoHD,
                        tenKH = hoaDon.KhachHang?.TenKH,
                        ptttCu = ptttCu?.TenPT ?? "Không có",
                        ptttMoi = ptttMoi.TenPT
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật phương thức thanh toán cho {danhSachSoHD.Count} hóa đơn.",
                updatedHoaDon = updateHoaDon
            });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XemChiTietSanPhamTheoHoaDon(List<int> selectedSoHD)
        {
            if (selectedSoHD == null || selectedSoHD.Count == 0)
            {
                return RedirectToAction("DanhSachHoaDon");
            }

            var allSanpham = (await _cthdRepository.GetAllAsync()).Where(ct => selectedSoHD.Contains(ct.SoHD)).Select(ct => ct.MaSP).Distinct().ToList();
            string tuKhoaTimKiem = "";
            bool dauTien = true;

            for (int i = 0; i < allSanpham.Count; i++)
            {
                if (!dauTien)
                {
                    tuKhoaTimKiem = tuKhoaTimKiem + ",";
                }
                tuKhoaTimKiem = tuKhoaTimKiem + allSanpham[i].ToString();
                dauTien = false;
            }

            if (tuKhoaTimKiem == "")
            {
                return RedirectToAction("Index", "SanPham", new {  tuKhoaTimKiem = "", chonTrangThai = "tatCa", chonDanhMuc = "" });
            }

            return RedirectToAction("Index", "SanPham", new
            {
                tuKhoaTimKiem = tuKhoaTimKiem + ",",
                chonTrangThai = "tatCa",
                chonDanhMuc = ""

            });
        }


        // Hiển thị chi tiết hóa đơn
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ChiTietHoaDon(int id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
    }
}