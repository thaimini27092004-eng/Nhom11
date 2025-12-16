using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Models.QuanLyTrangThai
{
    public class LichSuTTHD
    {
        [Key]
        public int MaLS { get; set; } // Mã lịch sử (khóa chính)

        public int SoHD { get; set; } // Số hóa đơn (khóa ngoại)

        public int MaTT { get; set; } // Mã trạng thái (khóa ngoại)

        [Required]
        public DateTime ThoiGianThayDoi { get; set; } // Thời gian thay đổi trạng thái

        public string? GhiChu { get; set; } // Ghi chú (tùy chọn)

        [ForeignKey("SoHD")]
        public HoaDon? HoaDon { get; set; } // Quan hệ với HoaDon

        [ForeignKey("MaTT")]
        public TrangThai? TrangThai { get; set; } // Quan hệ với TrangThai
        public List<GmailThongBao>? GmailThongBao { get; set; }

    }
}
