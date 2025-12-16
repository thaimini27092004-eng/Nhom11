using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface IDanhMucRepository
    {
        Task<IEnumerable<DanhMuc>> GetAllAsync();
        Task<IEnumerable<DanhMuc>> GetDisplayedAsync();
        Task<DanhMuc> GetByIdAsync(int maDM);
        Task AddAsync(DanhMuc danhmuc);
        Task UpdateAsync(DanhMuc danhmuc);
        Task DeleteAsync(int maDM);
    }
}
