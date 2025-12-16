using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface IKhachHangRepository
    {
        Task<IEnumerable<KhachHang>> GetAllAsync();
        Task<KhachHang> GetByIdAsync(int maKH);
        Task<KhachHang> GetByUserIdAsync(string userId);
        Task<List<int>> GetHoaDonIdsByUserIdAsync(string userId);
        Task<List<int>> GetHoaDonIdsByMaKHsAsync(List<int> maKHs); // Thêm phương thức mới
        Task AddAsync(KhachHang khachHang);
        Task UpdateAsync(KhachHang khachHang);
        Task DeleteAsync(int maKH);
    }
}