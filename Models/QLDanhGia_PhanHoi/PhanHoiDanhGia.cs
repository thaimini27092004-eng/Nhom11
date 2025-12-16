using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    public class PhanHoiDanhGia
    {
        [Key]
        public int MaPHDG { get; set; }
        public int MaKH { get; set; }
        public int MaDG { get; set; }
        [StringLength(500)]
        public string? NoiDungPHDG { get; set; }
        public DateTime? NgayPHDG { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaDG")]
        public DanhGia? DanhGia { get; set; }

    }
}
