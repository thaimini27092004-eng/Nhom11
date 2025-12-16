namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi.ViewModel
{
    public class TraLoiViewModel
    {
        public PhanHoiBinhLuan TraLoi { get; set; }
        public bool DaThich { get; set; }
        public int SoLuotThich { get; set; }
        public bool DaMuaSanPham { get; set; }
        public int TrangHienTai { get; set; } // Trang hiện tại
        public int TongSoTrang { get; set; } // Tổng số trang
        public int KichThuocTrang { get; set; } // Số trả lời mỗi trang
        public string NoiDungHienThi { get; set; } // Nội dung đã xử lý
    }
}
