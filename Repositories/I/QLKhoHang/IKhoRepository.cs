using WebsiteBanHang.Models.QLTonKho;

namespace WebsiteBanHang.Repositories.I.QLKhoHang
{
    public interface IKhoRepository
    {
        Task<IEnumerable<Kho>> GetAllAsync();
        Task<IEnumerable<Kho>> GetDisplayedAsync();
        Task<Kho> GetByIdAsync(int maKho);
        Task AddAsync(Kho kho);
        Task UpdateAsync(Kho kho);
        Task DeleteAsync(int maKho);
    }
}
