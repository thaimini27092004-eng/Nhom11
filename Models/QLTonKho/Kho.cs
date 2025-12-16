using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Models.QLTonKho
{
    public class Kho
    {
        [Key]

        public int MaKho { get; set; }
        [StringLength(50)]
        public String? TenKho { get; set; }
        public bool TTHienThi { get; set; } = true; // Mặc định hiển thị
        [StringLength(200)]
        public String? DCKho { get; set; }
        public List<TonKho>? TonKho { get; set; }

    }
}
