using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi
{
    public interface ILikePhanHoiDanhGiaRepository
    {
        Task<IEnumerable<LikePhanHoiDanhGia>> GetAllAsync();
        Task<LikePhanHoiDanhGia> GetByIdAsync(int maKH, int maPHDG);
        Task<IEnumerable<LikePhanHoiDanhGia>> GetByMaPHDGAsync(int maPHDG);
        Task<IEnumerable<LikePhanHoiDanhGia>> GetByMaKHAsync(int maKH);
        Task AddAsync(LikePhanHoiDanhGia likePhanHoiDanhGia);
        Task DeleteAsync(int maKH, int maPHDG);
        Task<bool> ExistsAsync(int maKH, int maPHDG);
    }
}
