using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models.QuanLyBanner
{
    public class BannerQuangCao
    {
        [Key]
        public int MaAQC { get; set; }

        [Required]
        public string UrlAnh { get; set; }

        [Required]
        public string MoTa { get; set; }

        public string UrlDichDen { get; set; }

        public bool TTHienThi { get; set; } = true; 
    }
}
