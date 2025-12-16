using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models
{
    public class DanhMuc
    {
        [Key]
        public int MaDM { get; set; }
        [Required, StringLength(50)]
        public string TenDM { get; set; }
        public bool TTHienThi { get; set; } = true; // Mặc định hiển thị
        public bool TTDeXuat { get; set; } = false; // Mặc định hiển thị
        public List<SanPham>? SanPham { get; set; }
    }
}