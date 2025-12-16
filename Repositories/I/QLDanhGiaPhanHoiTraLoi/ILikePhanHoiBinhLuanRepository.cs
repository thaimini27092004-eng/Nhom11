using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface ILikePhanHoiBinhLuanRepository
    {
        // Lấy tất cả lượt like trả lời
        Task<IEnumerable<LikePhanHoiBinhLuan>> GetAllAsync();

        // Lấy lượt like theo MaKH và MaTL
        Task<LikePhanHoiBinhLuan> GetByIdAsync(int maKH, int maTL);

        // Lấy danh sách lượt like theo MaTL
        Task<IEnumerable<LikePhanHoiBinhLuan>> GetByMaTLAsync(int maTL);

        // Lấy danh sách lượt like theo MaKH
        Task<IEnumerable<LikePhanHoiBinhLuan>> GetByMaKHAsync(int maKH);

        // Thêm lượt like mới
        Task AddAsync(LikePhanHoiBinhLuan likeTraLoi);

        // Xóa lượt like
        Task DeleteAsync(int maKH, int maTL);

        // Kiểm tra xem khách hàng đã like trả lời chưa
        Task<bool> ExistsAsync(int maKH, int maTL);
    }
}