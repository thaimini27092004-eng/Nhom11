namespace WebsiteBanHang.Models.ViewModel
{
    public class PhanTrangViewModel
    {
        public int TrangHienTai { get; set; }
        public int TongSoTrang { get; set; }
        public string DuongDan { get; set; } // URL như /Home/GetPhanHoiByMaSP
        public int MaDoiTuong { get; set; } // maSP, maPH, maDanhMuc
        public string ThamSoDoiTuong { get; set; } // "maSP", "maPH", "maDanhMuc"
    }
}
