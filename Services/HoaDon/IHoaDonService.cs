using WebsiteBanHang.Models.GioHang;

namespace WebsiteBanHang.Services.HoaDon
{
    public interface IHoaDonService
    {
        Task<int> TaoHoaDonVaGiamTonKhoAsync(int maKH, int maPT, List<CartItem> danhSachSanPhamDaChon, string diaChiGiaoHang, string dtNhanHang);
        Task XoaSanPhamKhoiGioHangAsync(int maKH, List<int> danhSachMaSanPham);
    }
}
