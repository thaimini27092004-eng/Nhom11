using WebsiteBanHang.Models.QLTonKho;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebsiteBanHang.Repositories.I
{
    public interface ITonKhoRepository
    {
        Task<IEnumerable<TonKho>> GetAllAsync();
        Task<TonKho> GetByIdAsync(int maKho, int maSP);
        Task<IEnumerable<TonKho>> GetByMaSPAsync(int maSP);

        Task AddAsync(TonKho tonKho);
        Task UpdateAsync(TonKho tonKho);
        Task DeleteAsync(int maKho, int maSP);
        Task<TonKho> FindKhoWithEnoughStockAsync(int maSP, int quantity);
    }
}