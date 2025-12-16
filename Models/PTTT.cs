

using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models
{
    public class PTTT
    {
        [Key] // Đánh dấu là khóa chính
        public int MaPT { get; set; } // Khóa chính

        [Required, StringLength(50)]
        public string TenPT { get; set; }
        public bool TTHienThi { get; set; } = true; // Mặc định hiển thị

        public List<HoaDon>? HoaDons { get; set; } // Mối quan hệ 1-n với HoaDon
    }
}