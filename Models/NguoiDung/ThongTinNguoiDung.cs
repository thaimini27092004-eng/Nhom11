using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebsiteBanHang.Models.NguoiDung
{
    public class ThongTinNguoiDung : IdentityUser
    {
        [Required]
        public string HoTen { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? SDT { get; set; }

    }
}
