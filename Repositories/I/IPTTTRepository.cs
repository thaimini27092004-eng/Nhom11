using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface IPTTTRepository
    {
        Task<IEnumerable<PTTT>> GetAllAsync();
        Task<PTTT> GetByIdAsync(int maPT);
        Task AddAsync(PTTT pttt);
        Task UpdateAsync(PTTT pttt);
        Task DeleteAsync(int maPT);
    }
}