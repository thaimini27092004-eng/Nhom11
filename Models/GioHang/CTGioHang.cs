using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Models.GioHang
{
    [PrimaryKey(nameof(MaGH), nameof(MaSP))] // Định nghĩa khóa chính composite
    public class CTGioHang
    {
        [Column(Order = 0)]
        public int MaGH { get; set; } // Khóa ngoại đến LuuGioHang

        [ Column(Order = 1)]
        public int MaSP { get; set; } // Khóa ngoại đến Product
        public int SoLuongThem { get; set; } // Số lượng, bắt buộc > 0

        public long GiaLucThem { get; set; } // Giá tại thời điểm thêm
        public DateTime ThoiGianThemDau { get; set; } // Đổi từ ThoiGianThem thành ThoiGianThemDau
        public long GiaThemCuoi { get; set; } // Giá tại thời điểm cập nhật cuối
        public DateTime ThoiGianThemCuoi { get; set; } // Thêm cột ThoiGianThemCuoi
       // [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public bool TrangThaiChon { get; set; } // Thêm cột TrangThaiChon kiểu bool

        [ForeignKey("MaGH")]
        public GioHang GioHang { get; set; } // Quan hệ với LuuGioHang

        [ForeignKey("MaSP")]
        public SanPham SanPham { get; set; } // Quan hệ với Product
    }
}