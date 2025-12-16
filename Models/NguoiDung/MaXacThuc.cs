using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models.NguoiDung
{
    public class MaXacThuc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MaXacNhan { get; set; }

        [Required]
        public DateTime ThoiGianTao { get; set; }

        [Required]
        public DateTime ThoiGianHetHan { get; set; }

        public bool DaSuDung { get; set; }
        public int SoLanThu { get; set; } // Số lần thử sai
    }
}
