using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLTonKho
{

    [PrimaryKey(nameof(MaKho), nameof(MaSP))] // Định nghĩa khóa chính composite
    public class TonKho
    {
        public int MaKho { get; set; }
        public int MaSP { get; set; }
        public int SLTon {  get; set; }
        public bool TTHienThi { get; set; } = true; // Mặc định hiển thị

        [ForeignKey("MaKho")]
        public Kho? Kho { get; set; }
        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }

        public List<CTHD>? CTHDs { get; set; }

    }
}
