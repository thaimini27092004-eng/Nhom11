namespace WebsiteBanHang.Models.QuanLyHoaDon.ViewModel
{
    public class HoaDonViewModel
    {
        public int SoHD { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenKH { get; set; }
        public long TongTien { get; set; }
        public string TenPT { get; set; }
        public string TenTT { get; set; }
        public List<int> MaKhoList { get; set; } // Danh sách các MaKho từ CTHD
    }
}
