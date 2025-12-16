using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Models.GioHang
{
    public class GioHang
    {
        [Key]
        public int MaGH { get; set; } // Khóa chính

        [Required]
        public int MaKH { get; set; } // Khóa ngoại đến KhachHang

        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; } // Quan hệ với KhachHang

        public List<CTGioHang>? CTGioHangs { get; set; }
    }
}