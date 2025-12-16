using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebsiteBanHang.Models
{
    public class DSAnh
    {
        [Key]
        public int MaAnh { get; set; }
        public string UrlAnh { get; set; }
        public int MaSP { get; set; }
        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }
    }
}
