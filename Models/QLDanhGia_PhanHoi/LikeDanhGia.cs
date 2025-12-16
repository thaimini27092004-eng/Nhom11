using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    [PrimaryKey(nameof(MaKH), nameof(MaDG))] // Định nghĩa khóa chính composite

    public class LikeDanhGia
    {
        public int MaDG { get; set; }
        public int MaKH { get; set; }
        public DateTime? TGThich { get; set; }
        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        [ForeignKey("MaDG")]
        public DanhGia? DanhGia { get; set; }


    }
}
