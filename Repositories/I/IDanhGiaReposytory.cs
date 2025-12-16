using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface IDanhGiaRepository
    {
        Task<IEnumerable<DanhGia>> GetAllAsync();
        Task<DanhGia> GetByIdAsync(int maDG);
        Task<IEnumerable<DanhGia>> GetByMaSPPagedAsync(int maSP, int pageNumber, int pageSize);
        Task<IEnumerable<DanhGia>> GetByMaKHAsync(int maKH);
        Task AddAsync(DanhGia danhGia);
        Task UpdateAsync(DanhGia danhGia);
        Task DeleteAsync(int maDG);
        Task<bool> ExistsAsync(int soHD, int maSP, int maKho);
    }
}