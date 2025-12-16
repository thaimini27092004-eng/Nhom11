using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Models.QLTonKho;

namespace WebsiteBanHang.Models
{
    [PrimaryKey(nameof(SoHD), nameof(MaSP), nameof(MaKho))] // Định nghĩa khóa chính composite
    public class CTHD
    {
        [Column(Order = 0)]
        public int SoHD { get; set; } // Khóa ngoại đến HoaDon

        [ Column(Order = 1)]
        public int MaSP { get; set; }// Khóa ngoại đến Sản phẩm
        public int MaKho { get; set; }// Khóa ngoại đến kho hàng
        public int SL { get; set; } // Số lượng
        public long DonGia { get; set; } // Đơn giá


        [ForeignKey("SoHD")]
        public HoaDon? HoaDon { get; set; }

        [ForeignKey("MaKho,MaSP")]
        public TonKho? TonKho { get; set; }

    }
    
}

