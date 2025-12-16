using WebsiteBanHang.Models.GioHang;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QLTonKho;
using WebsiteBanHang.Models.QuanLyTrangThai;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Services.GuiEmailTHongBao;

namespace WebsiteBanHang.Services.HoaDon
{
    

    public class HoaDonService : IHoaDonService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IGioHangRepository _khoLuuGioHang;
        private readonly ICTGioHangRepository _khoChiTietGioHang;
        private readonly ITonKhoRepository _tonKhoRepository; 
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;
        private readonly IEmailService _emailService;

        public HoaDonService(
            ApplicationDbContext dbContext,
            IGioHangRepository khoLuuGioHang,
            ICTGioHangRepository khoChiTietGioHang,
            ITonKhoRepository tonKhoRepository,
            ILichSuTTHDRepository lichSuTTHDRepository,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _khoLuuGioHang = khoLuuGioHang;
            _khoChiTietGioHang = khoChiTietGioHang;
            _tonKhoRepository = tonKhoRepository;
            _lichSuTTHDRepository = lichSuTTHDRepository;
            _emailService = emailService;
        }

        public async Task<int> TaoHoaDonVaGiamTonKhoAsync(int maKH, int maPT, List<CartItem> danhSachSanPhamDaChon, string diaChiGiaoHang, string dtNhanHang)
        {
            var hoaDon = new WebsiteBanHang.Models.HoaDon
            {
                NgayDat = DateTime.Now,
                NgayGiaoDuKien = DateTime.Now.AddDays(3),
                MaPT = maPT,
                MaKH = maKH,
                DiaChiGiaoHang = diaChiGiaoHang,
                DTNhanHang = dtNhanHang
            };
            _dbContext.HoaDon.Add(hoaDon);
            await _dbContext.SaveChangesAsync();

            // Thêm bản ghi lịch sử trạng thái ban đầu (MaTT = 1)
            var lichSu = new LichSuTTHD
            {
                SoHD = hoaDon.SoHD,
                MaTT = 1, // Trạng thái mặc định
                ThoiGianThayDoi = DateTime.Now,
                GhiChu = "Tạo hóa đơn"
            };
            await _lichSuTTHDRepository.AddAsync(lichSu); // Sử dụng repository

            // Lấy danh sách MaSP và Quantity từ danhSachSanPhamDaChon
            var productRequirements = danhSachSanPhamDaChon
                .Select(sp => new { MaSP = sp.ProductId, Quantity = sp.Quantity })
                .ToList();

            // Bước 1: Tìm kho chung chứa tất cả sản phẩm với SLTon đủ
            var tonKhoList = await _tonKhoRepository.GetAllAsync();
            var khoChung = tonKhoList
                .GroupBy(tk => tk.MaKho)
                .Where(g => productRequirements.All(req =>
                    g.Any(tk => tk.MaSP == req.MaSP && tk.SLTon >= req.Quantity)))
                .Select(g => g.Key)
                .FirstOrDefault();

            // Lưu trữ các kho được chọn cho từng sản phẩm
            var khoSanPham = new Dictionary<int, (int MaKho, TonKho TonKho)>();

            foreach (var sanPhamDaChon in danhSachSanPhamDaChon)
            {
                var sanPham = await _dbContext.SanPham.FindAsync(sanPhamDaChon.ProductId);
                if (sanPham != null)
                {
                    // Bước 2: Tìm bản ghi TonKho cho sản phẩm
                    var tonKho = khoChung != 0
                        ? tonKhoList.FirstOrDefault(tk => tk.MaKho == khoChung && tk.MaSP == sanPhamDaChon.ProductId && tk.SLTon >= sanPhamDaChon.Quantity)
                        : null;

                    // Nếu không tìm thấy ở kho chung hoặc kho chung không tồn tại, tìm kho khác
                    if (tonKho == null)
                    {
                        tonKho = await _tonKhoRepository.FindKhoWithEnoughStockAsync(sanPhamDaChon.ProductId, sanPhamDaChon.Quantity);
                    }

                    // Bước 3: Nếu không tìm thấy kho nào có đủ SLTon, bỏ qua toàn bộ đơn hàng
                    if (tonKho == null)
                    {
                        // Xóa hóa đơn vừa tạo vì không tạo được bất kỳ CTHD nào
                        _dbContext.HoaDon.Remove(hoaDon);
                        await _dbContext.SaveChangesAsync();
                        return hoaDon.SoHD; // Trả về SoHD nhưng không có CTHD
                    }

                    // Lưu thông tin kho cho sản phẩm
                    khoSanPham[sanPhamDaChon.ProductId] = (tonKho.MaKho, tonKho);
                }
            }

            // Bước 4: Tạo CTHD và giảm SLTon cho từng sản phẩm
            foreach (var sanPhamDaChon in danhSachSanPhamDaChon)
            {
                var sanPham = await _dbContext.SanPham.FindAsync(sanPhamDaChon.ProductId);
                if (sanPham != null && khoSanPham.TryGetValue(sanPhamDaChon.ProductId, out var kho))
                {
                    // Tạo chi tiết hóa đơn
                    var chiTietHoaDon = new CTHD
                    {
                        SoHD = hoaDon.SoHD,
                        MaSP = sanPham.MaSP,
                        MaKho = kho.MaKho, // Lưu MaKho từ TonKho vào CTHD
                        SL = sanPhamDaChon.Quantity,
                        DonGia = sanPham.Gia
                    };
                    _dbContext.CTHD.Add(chiTietHoaDon);

                    // Giảm số lượng tồn kho
                    kho.TonKho.SLTon -= sanPhamDaChon.Quantity;
                    await _tonKhoRepository.UpdateAsync(kho.TonKho); // Sử dụng repository để cập nhật
                }
            }
            await _dbContext.SaveChangesAsync();

            // Gửi email thông báo đặt hàng thành công
            var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.MaKH == maKH);
            if (khachHang != null && !string.IsNullOrEmpty(khachHang.EmailKH))
            {
                await _emailService.GuiEmailDatHangThanhCongAsync(hoaDon.SoHD, khachHang.EmailKH);
            }

            return hoaDon.SoHD;
        }

        public async Task XoaSanPhamKhoiGioHangAsync(int maKH, List<int> danhSachMaSanPham)
        {
            var gioHang = await _khoLuuGioHang.GetOrCreateGioHangAsync(maKH);
            var danhSachSanPhamTrongGio = await _khoChiTietGioHang.GetCartItemsAsync(gioHang.MaGH);
            foreach (var sanPhamTrongGio in danhSachSanPhamTrongGio.Where(ci => danhSachMaSanPham.Contains(ci.MaSP)))
            {
                await _khoChiTietGioHang.DeleteCartItemAsync(gioHang.MaGH, sanPhamTrongGio.MaSP);
            }
        }
    }
}
