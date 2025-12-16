using WebsiteBanHang.Models.QuanLyTrangThai;

namespace WebsiteBanHang.Repositories.I.QLTrangThai
{
    public interface ILichSuTTHDRepository
    {
        Task<IEnumerable<LichSuTTHD>> GetAllAsync();
        Task<LichSuTTHD> GetByIdAsync(int maLS);
        Task<IEnumerable<LichSuTTHD>> GetBySoHDAsync(int soHD);
        Task<LichSuTTHD> GetLatestBySoHDAsync(int soHD);
        Task AddAsync(LichSuTTHD lichSu);
        Task UpdateAsync(LichSuTTHD lichSu);
        Task DeleteAsync(int maLS);
    }
}
