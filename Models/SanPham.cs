using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebsiteBanHang.Models.GioHang;
using WebsiteBanHang.Models.QLTonKho;

namespace WebsiteBanHang.Models
{
    public class SanPham
    {
        [Key]
        public int MaSP { get; set; }
        public int MaDM { get; set; }

        [Required, StringLength(100)]
        public string TenSP { get; set; }

        [Range(0.01, 100000000.00)]
        public long Gia { get; set; }
        public string MoTa { get; set; }
        public string? UrlAnh { get; set; }
        public bool TTDeXuat { get; set; } // Thuộc tính để admin chọn sản phẩm đề xuất
        public bool TTHienThi { get; set; } = true;

        [ForeignKey("MaDM")]
        public DanhMuc? DanhMuc { get; set; }

        public List<CTGioHang>? CTGioHangs { get; set; }

        public List<DSAnh>? DSAnh { get; set; }
        public List<TonKho>? TonKho { get; set; }


    }
}
