using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    [PrimaryKey(nameof(MaKH), nameof(MaBL))] // Định nghĩa khóa chính composite

    public class LikeBinhLuan
    {
        public int MaKH { get; set; }
        public int MaBL { get; set; }
        public DateTime? TGThich { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaPH")]
        public BinhLuan? BinhLuan { get; set; }


    }
}
