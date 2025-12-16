using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    [PrimaryKey(nameof(MaKH), nameof(MaPHBL))] // Định nghĩa khóa chính composite

    public class LikePhanHoiBinhLuan
    {
        public int MaPHBL { get; set; }
        public int MaKH { get; set; }
        public DateTime? TGThich { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaTL")]
        public PhanHoiBinhLuan? TraLoi { get; set; }


    }
}
