using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QLGmailThongBao;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;

namespace WebsiteBanHang.Services.GuiEmailTHongBao
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IQuanLyHoaDonRepository _hoaDonRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;
        private readonly IGmailThongBaoRepository _gmailThongBaoRepository;
        private readonly string _baseUrl;

        public EmailService(
            IEmailSender emailSender,
            IQuanLyHoaDonRepository hoaDonRepository,
            IKhachHangRepository khachHangRepository,
            ILichSuTTHDRepository lichSuTTHDRepository,
            IGmailThongBaoRepository gmailThongBaoRepository,
            IConfiguration configuration)
        {
            _emailSender = emailSender;
            _hoaDonRepository = hoaDonRepository;
            _khachHangRepository = khachHangRepository;
            _lichSuTTHDRepository = lichSuTTHDRepository;
            _gmailThongBaoRepository = gmailThongBaoRepository;
            _baseUrl = configuration["BaseUrl"] ?? "http://localhost:5143";
        }

        // Đọc và thay thế nội dung template
        private async Task<string> TaoNoiDungEmailTuTemplateAsync(string tenTemplate, Dictionary<string, string> duLieu)
        {
            var duongDanTemplate = Path.Combine("wwwroot", "email_templates", $"{tenTemplate}.html");
            if (!File.Exists(duongDanTemplate))
            {
                return "<p>Template không tồn tại.</p>";
            }

            var noiDungTemplate = await File.ReadAllTextAsync(duongDanTemplate);
            var noiDungHtml = noiDungTemplate;

            for (int i = 0; i < duLieu.Count; i++)
            {
                var item = duLieu.ElementAt(i);
                noiDungHtml = noiDungHtml.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return noiDungHtml;
        }

        // Gửi email chung
        public async Task GuiEmailAsync(string emailNguoiNhan, string tieuDe, string noiDungHtml)
        {
            try
            {
                await _emailSender.SendEmailAsync(emailNguoiNhan, tieuDe, noiDungHtml);

                var thongBao = new GmailThongBao
                {
                    MaLS = 0,
                    NoiDung = noiDungHtml,
                    ThoiGianGui = DateTime.Now
                };
                await _gmailThongBaoRepository.ThemThongBaoAsync(thongBao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
            }
        }

        // Gửi email thông báo đặt hàng thành công
        public async Task GuiEmailDatHangThanhCongAsync(int soHD, string emailKhachHang)
        {
            // Lấy hóa đơn qua repository
            var hoaDon = await _hoaDonRepository.GetByIdAsync(soHD);
            if (hoaDon == null)
            {
                return;
            }

            // Lấy khách hàng qua repository
            var khachHang = await _khachHangRepository.GetByIdAsync(hoaDon.MaKH);
            if (khachHang == null)
            {
                return;
            }

            // Tạo danh sách sản phẩm HTML
            string danhSachSanPhamHtml = "";
            if (hoaDon.CTHDs != null)
            {
                for (int i = 0; i < hoaDon.CTHDs.Count; i++)
                {
                    var ct = hoaDon.CTHDs.ElementAt(i);
                    var tenSP = ct.TonKho?.SanPham?.TenSP ?? "Sản phẩm";
                    var soLuong = ct.SL;
                    var donGia = ct.DonGia.ToString("N0");
                    var thanhTien = (ct.SL * ct.DonGia).ToString("N0");
                    var urlAnh = ct.TonKho?.SanPham?.UrlAnh ?? (ct.TonKho?.SanPham?.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/50");
                    if (!urlAnh.StartsWith("http"))
                    {
                        urlAnh = $"{_baseUrl}{urlAnh}";
                    }

                    danhSachSanPhamHtml += $@"
                        <tr>
                            <td style=""text-align: left; font-size: 14px; color: #333333; border-bottom: 1px solid #dddddd;"">
                                <img src=""{urlAnh}"" alt=""{tenSP}"" style=""width: 50px; vertical-align: middle; margin-right: 10px;"">
                            </td>
                            <td style=""text-align: left; font-size: 14px; color: #333333; border-bottom: 1px solid #dddddd;"">{tenSP}</td>
                            <td style=""text-align: center; font-size: 14px; color: #333333; border-bottom: 1px solid #dddddd;"">{soLuong}</td>
                            <td style=""text-align: right; font-size: 14px; color: #333333; border-bottom: 1px solid #dddddd;"">{donGia} VND</td>
                            <td style=""text-align: right; font-size: 14px; color: #333333; border-bottom: 1px solid #dddddd;"">{thanhTien} VND</td>
                        </tr>";
                }
            }

            // Tạo dữ liệu cho template
            var duLieu = new Dictionary<string, string>
            {
                { "TenKH", khachHang.TenKH ?? "Khách hàng" },
                { "SoHD", hoaDon.SoHD.ToString() },
                { "NgayDat", hoaDon.NgayDat.ToString("dd/MM/yyyy") },
                { "PhuongThucThanhToan", hoaDon.PTTT?.TenPT ?? "Không xác định" },
                { "DiaChiGiaoHang", hoaDon.DiaChiGiaoHang ?? "Không có" },
                { "DanhSachSanPham", danhSachSanPhamHtml },
                { "TongTien", hoaDon.CTHDs?.Sum(ct => ct.SL * ct.DonGia).ToString("N0") ?? "0" }
            };

            // Tạo nội dung HTML từ template
            var noiDungHtml = await TaoNoiDungEmailTuTemplateAsync("DatHangThanhCong", duLieu);

            // Lưu thông tin email vào GmailThongBao
            var latestLichSu = await _lichSuTTHDRepository.GetLatestBySoHDAsync(soHD);

            var thongBao = new GmailThongBao
            {
                MaLS = latestLichSu?.MaLS ?? 0,
                NoiDung = noiDungHtml,
                ThoiGianGui = DateTime.Now
            };

            try
            {
                await _emailSender.SendEmailAsync(emailKhachHang, "Thông báo đặt hàng thành công", noiDungHtml);
                await _gmailThongBaoRepository.ThemThongBaoAsync(thongBao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email đặt hàng: {ex.Message}");
            }
        }

        // Gửi email thông báo cập nhật trạng thái
        public async Task GuiEmailCapNhatTrangThaiAsync(int soHD, string emailKhachHang, string tenTrangThaiMoi)
        {
            // Lấy hóa đơn qua repository
            var hoaDon = await _hoaDonRepository.GetByIdAsync(soHD);
            if (hoaDon == null)
            {
                return;
            }

            // Lấy khách hàng qua repository
            var khachHang = await _khachHangRepository.GetByIdAsync(hoaDon.MaKH);
            if (khachHang == null)
            {
                return;
            }

            // Tạo dữ liệu cho template
            var duLieu = new Dictionary<string, string>
            {
                { "TenKH", khachHang.TenKH ?? "Khách hàng" },
                { "SoHD", hoaDon.SoHD.ToString() },
                { "TrangThaiMoi", tenTrangThaiMoi }
            };

            // Tạo nội dung HTML từ template
            var noiDungHtml = await TaoNoiDungEmailTuTemplateAsync("CapNhatTrangThai", duLieu);

            // Lưu thông tin email vào GmailThongBao
            var latestLichSu = await _lichSuTTHDRepository.GetLatestBySoHDAsync(soHD);

            var thongBao = new GmailThongBao
            {
                MaLS = latestLichSu?.MaLS ?? 0,
                NoiDung = noiDungHtml,
                ThoiGianGui = DateTime.Now
            };

            try
            {
                await _emailSender.SendEmailAsync(emailKhachHang, "Thông báo cập nhật trạng thái đơn hàng", noiDungHtml);
                await _gmailThongBaoRepository.ThemThongBaoAsync(thongBao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email cập nhật trạng thái: {ex.Message}");
            }
        }
    }
}
