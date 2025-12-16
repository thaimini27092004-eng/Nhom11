using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi.ViewModel
{
    public class PhanHoiViewModel
    {
        public BinhLuan BinhLuan { get; set; }
        public int SoLuongTraLoi { get; set; }
        public bool DaThich { get; set; }
        public int SoLuotThich { get; set; }
        public bool DaMuaSanPham { get; set; }
        public int TrangHienTai { get; set; } // Trang hiện tại
        public int TongSoTrang { get; set; } // Tổng số trang
        public int KichThuocTrang { get; set; } // Số bình luận mỗi trang
    }
}
