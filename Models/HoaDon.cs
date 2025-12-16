
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models.QuanLyTrangThai;

namespace WebsiteBanHang.Models
{
    public class HoaDon
    {
        [Key] // Đánh dấu là khóa chính
        public int SoHD { get; set; }
        public int MaPT { get; set; }
        public int MaKH { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayGiaoDuKien { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? DTNhanHang { get; set; }
        public int SoLanDoiTT { get; set; } = 0;
        [ForeignKey("MaPT")]
        public PTTT? PTTT { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        public List<CTHD>? CTHDs { get; set; }
        public List<LichSuTTHD>? LichSuTTHD {  get; set; }
    }
}
