using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface ITrangThaiRepository
    {
        Task<IEnumerable<TrangThai>> GetAllAsync();
        Task<TrangThai> GetByIdAsync(int maTT);
        Task AddAsync(TrangThai trangThai);
        Task UpdateAsync(TrangThai trangThai);
        Task DeleteAsync(int maTT);
    }
}