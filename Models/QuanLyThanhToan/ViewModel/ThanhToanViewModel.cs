using WebsiteBanHang.Models.GioHang;

namespace WebsiteBanHang.Models.QuanLyThanhToan.ViewModel
{
    public class ThanhToanViewModel
    {
        public List<CartItem> DanhSachSanPhamDaChon { get; set; }
        public List<PTTT> DanhSachPhuongThucThanhToan { get; set; }
        public string DiaChiGiaoHang { get; set; } // Địa chỉ giao hàng
        public string SoDienThoai { get; set; }    // Số điện thoại
    }
}
