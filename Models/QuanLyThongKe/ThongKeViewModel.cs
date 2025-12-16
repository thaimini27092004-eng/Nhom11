namespace WebsiteBanHang.Models.QuanLyThongKe
{
    public class ThongKeViewModel
    {
        public string NgayDatHang { get; set; } // Ngày đặt hàng (định dạng yyyy-MM-dd)
        public int SoLuongBan { get; set; } // Tổng số lượng sản phẩm bán
        public int SoLuongDonHang { get; set; } // Số lượng đơn hàng
        public double DoanhThu { get; set; } // Tổng doanh thu
    }
}
