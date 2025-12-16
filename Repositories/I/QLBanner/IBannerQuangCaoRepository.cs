using WebsiteBanHang.Models.QuanLyBanner;

namespace WebsiteBanHang.Repositories.I.QLBanner
{
    public interface IBannerQuangCaoRepository
    {
        Task<IEnumerable<BannerQuangCao>> GetAllAsync();
        Task<BannerQuangCao> GetByIdAsync(int id);
        Task AddAsync(BannerQuangCao banner);
        Task UpdateAsync(BannerQuangCao banner);
        Task DeleteAsync(int id);
    }
}