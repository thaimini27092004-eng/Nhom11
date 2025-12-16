

using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models.QuanLyTrangThai;

namespace WebsiteBanHang.Models
{
    public class TrangThai
    {
        [Key] // Đánh dấu là khóa chính
        public int MaTT { get; set; } // Khóa chính

        [Required, StringLength(50)]
        public string TenTT { get; set; }
        public string UrlAnh { get; set; }

        public bool TTHienThi { get; set; } = true; // Mặc định hiển thị

        public List<LichSuTTHD>? LichSuTTHD { get; set; } // Mối quan hệ 1-n với Lịch sử hóa đơn
    }
}