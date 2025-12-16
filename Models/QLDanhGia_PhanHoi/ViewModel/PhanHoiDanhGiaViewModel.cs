namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi.ViewModel
{
    public class PhanHoiDanhGiaViewModel
    {
        public PhanHoiDanhGia PhanHoiDanhGia { get; set; }
        public bool DaMuaSanPham { get; set; }
        public bool DaThich { get; set; } // Khách hàng hiện tại đã thích chưa
        public int SoLuotThich { get; set; } // Tổng số lượt thích
        public int TrangHienTai { get; set; }
        public int TongSoTrang { get; set; }
        public int KichThuocTrang { get; set; }
        public string NoiDungHienThi { get; set; }
    }
}
