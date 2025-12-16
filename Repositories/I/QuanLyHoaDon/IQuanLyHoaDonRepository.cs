using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I.QuanLyHoaDon
{
    public interface IQuanLyHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetAllAsync();
        Task<HoaDon> GetByIdAsync(int soHD);
        Task UpdateAsync(HoaDon hoaDon);
        Task DeleteAsync(int soHD);
        Task<IEnumerable<int>> GetMaSPByHoaDonAsync(List<int> soHD);
    }
}
