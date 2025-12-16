using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Models
{
    public class NhanVien
    {
        [Key] // Đánh dấu là khóa chính
        public int MaNV { get; set; } // Khóa chính
        public string UserId { get; set; } // Khóa ngoại trỏ đến AspNetUsers (ThongTinNguoiDung

        [Required, StringLength(100)]
        public string TenNV { get; set; }

        public DateTime? NgaySinhNV { get; set; }
        public string? DiaChiNV { get; set; }

        public string? DTNV { get; set; }
        public string? EmailNV { get; set; }
        public string? AvatarUrl { get; set; }

        [ForeignKey("UserId")]
        public ThongTinNguoiDung User { get; set; } // Quan hệ 1-1 với ThongTinNguoiDung
       
    }
}
