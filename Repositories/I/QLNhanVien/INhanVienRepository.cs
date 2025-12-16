using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I.QLNhanVien
{
    public interface INhanVienRepository
    {
        Task<NhanVien> GetByUserIdAsync(string userId);
        Task AddAsync(NhanVien nhanVien);
        Task DeleteAsync(int maNV);
        Task<bool> ExistsAsync(string userId);
    }
}
