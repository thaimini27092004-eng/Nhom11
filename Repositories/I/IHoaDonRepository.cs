using WebsiteBanHang.Models;
using WebsiteBanHang.Models.QuanLyThongKe;

namespace WebsiteBanHang.Repositories.I
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetAllAsync(); // Lấy tất cả hóa đơn
        Task<HoaDon> GetByIdAsync(int soHD); // Lấy hóa đơn theo số hóa đơn
        Task AddAsync(HoaDon hoadon); // Thêm hóa đơn mới
        Task UpdateAsync(HoaDon hoadon); // Cập nhật hóa đơn
        Task DeleteAsync(int soHD); // Xóa hóa đơn theo số hóa đơn
    }
}

