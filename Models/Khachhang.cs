
using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models.GioHang;
using System.Collections.Generic;
using WebsiteBanHang.Models.NguoiDung;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;


namespace WebsiteBanHang.Models
{
    public class KhachHang
    {
        [Key] // Đánh dấu là khóa chính
        public int MaKH { get; set; } // Khóa chính
        public string UserId { get; set; } // Khóa ngoại trỏ đến AspNetUsers (ThongTinNguoiDung

        [Required, StringLength(100)]
        public string TenKH { get; set; }

        public DateTime? NgaySinhKH { get; set; }
        public string? DiaChiKH { get; set; }

        public string? DTKH { get; set; }
        public string? EmailKH { get; set; }
        public string? AvatarUrl { get; set; } 

        [ForeignKey("UserId")]
        public ThongTinNguoiDung User { get; set; } // Quan hệ 1-1 với ThongTinNguoiDung
        public List<HoaDon>? HoaDons { get; set; } // Mối quan hệ 1-n với HoaDon

        public GioHang.GioHang GioHang { get; set; } // Navigation property trỏ đến GioHang
    }
}