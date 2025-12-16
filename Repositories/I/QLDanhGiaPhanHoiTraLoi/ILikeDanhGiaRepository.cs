using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface ILikeDanhGiaRepository
    {
        Task<IEnumerable<LikeDanhGia>> GetAllAsync();
        Task<LikeDanhGia> GetByIdAsync(int maKH, int maDG);
        Task<IEnumerable<LikeDanhGia>> GetByMaDGAsync(int maDG);
        Task<IEnumerable<LikeDanhGia>> GetByMaKHAsync(int maKH);
        Task AddAsync(LikeDanhGia likeDanhGia);
        Task DeleteAsync(int maKH, int maDG);
        Task<bool> ExistsAsync(int maKH, int maDG);
    }
}