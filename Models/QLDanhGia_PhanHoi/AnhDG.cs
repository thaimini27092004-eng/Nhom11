using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    public class AnhDG
    {
        [Key]
        public int MaAnhDG { get; set; }
        public int MaDG {  get; set; }
        bool TTHienThi { get; set; } = true;

        [ForeignKey("MaDG")]
        public DanhGia? DanhGia { get; set; }

    }
}
