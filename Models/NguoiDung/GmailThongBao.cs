using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteBanHang.Models.QuanLyTrangThai;

namespace WebsiteBanHang.Models.NguoiDung
{
    public class GmailThongBao
    {
        [Key]
        public int MaTB { get; set; }

        public int MaLS { get; set; }

        [Required]
        public string NoiDung { get; set; }

        [Required]
        public DateTime ThoiGianGui { get; set; }

        [ForeignKey("MaLS")]
        public LichSuTTHD? LichSuTTHD { get; set; } // Quan hệ với HoaDon

    }
}
