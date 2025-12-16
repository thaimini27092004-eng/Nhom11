using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteBanHang.Models.QLDanhGia_PhanHoi
{
    [PrimaryKey(nameof(MaDG))]
    public class DanhGia
    {
        public int MaDG { get; set; }
        public int MaKho { get; set; }
        public int MaSP { get; set; }
        public int SoHD { get; set; }
        [StringLength(300)]
        public string? NoiDung { get; set; }
        [Range(1, 5)]
        public int Sao { get; set; }
        public DateTime? NgayDG { get; set; }
        public bool? TTHienThi { get; set; } = true;

        [ForeignKey("SoHD, MaSP, MaKho")]
        public CTHD? CTHD { get; set; }

    }
}
