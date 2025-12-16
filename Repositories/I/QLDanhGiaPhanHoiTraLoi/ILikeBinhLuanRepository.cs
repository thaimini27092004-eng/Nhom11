using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface ILikeBinhLuanRepository
    {
        // Lấy tất cả lượt like phản hồi
        Task<IEnumerable<LikeBinhLuan>> GetAllAsync();

        // Lấy lượt like theo MaKH và MaPH
        Task<LikeBinhLuan> GetByIdAsync(int maKH, int maPH);

        // Lấy danh sách lượt like theo MaPH
        Task<IEnumerable<LikeBinhLuan>> GetByMaPHAsync(int maPH);

        // Lấy danh sách lượt like theo MaKH
        Task<IEnumerable<LikeBinhLuan>> GetByMaKHAsync(int maKH);

        // Thêm lượt like mới
        Task AddAsync(LikeBinhLuan likePhanHoi);

        // Xóa lượt like
        Task DeleteAsync(int maKH, int maPH);

        // Kiểm tra xem khách hàng đã like phản hồi chưa
        Task<bool> ExistsAsync(int maKH, int maPH);
    }
}