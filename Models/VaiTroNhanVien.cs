using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models
{
    public class VaiTroNhanVien
    {
        [Key]
        public int MaVTNV { get; set; }

        public int MaNV { get; set; }
        public string ChucVu { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; } // Mối quan hệ 1-n với nhân viên


    }
}
