using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    
    public class BinhLuan
    {
        [Key]
        public int MaBL { get; set; }
        public int MaKH { get; set; }
        public int MaSP { get; set; }
        [StringLength(500)]
        public string? NoiDungBL { get; set; }
        public DateTime? NgayBL { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }

    }
    
}
