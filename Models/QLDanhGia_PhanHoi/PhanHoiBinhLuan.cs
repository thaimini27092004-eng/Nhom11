using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{

    public class PhanHoiBinhLuan
    {
        [Key]
        public int MaPHBL { get; set; } // Khóa ngoại trỏ tới PhanHoi

        public int MaBL { get; set; } // Khóa ngoại trỏ tới PhanHoi

        public int MaKH { get; set; } // Khóa ngoại trỏ tới KhachHang

        [StringLength(500)]
        public string? NoiDungPHBL { get; set; } // Nội dung câu trả lời

        public DateTime? NgayPHBL { get; set; } // Ngày trả lời

        [ForeignKey("MaPH")]
        public BinhLuan? BinhLuan { get; set; } // Navigation property trỏ tới PhanHoi

        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; } // Navigation property trỏ tới KhachHang
    }

}
