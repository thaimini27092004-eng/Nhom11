using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    [PrimaryKey(nameof(MaKH), nameof(MaPHDG))] // Định nghĩa khóa chính composite

    public class LikePhanHoiDanhGia
    {
        public int MaKH { get; set; }
        public int MaPHDG { get; set; }
        public DateTime? TGThich { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaPHDG")]
        public PhanHoiDanhGia? PhanHoiDanhGia { get; set; }
    }
}
