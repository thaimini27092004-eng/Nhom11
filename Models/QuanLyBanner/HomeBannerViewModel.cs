namespace WebsiteBanHang.Models.QuanLyBanner
{
    public class HomeBannerViewModel
    {
        public List<SanPham> SanPhamsDeXuat { get; set; } = new List<SanPham>();
        public List<BannerQuangCao> BannerQuangCao { get; set; } = new List<BannerQuangCao>();
    }
}
