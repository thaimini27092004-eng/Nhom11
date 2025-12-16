using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface ISanPhamRepository
    {
        Task<IEnumerable<SanPham>> GetAllAsync();
        Task<SanPham> GetByIdAsync(int id);
        Task<List<SanPham>> GetByIdsAsync(List<int> ids);
        Task<IEnumerable<SanPham>> GetByMaKhoAsync(int maKho);
        Task AddAsync(SanPham sanpham);
        Task UpdateAsync(SanPham sanpahm);
        Task DeleteAsync(int id);
    }
}
