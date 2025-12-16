using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi
{
    public interface IPhanHoiDanhGiaRepository
    {
        Task<IEnumerable<PhanHoiDanhGia>> GetAllAsync();
        Task<PhanHoiDanhGia> GetByIdAsync(int maPHDG);
        Task<IEnumerable<PhanHoiDanhGia>> GetByMaDGPagedAsync(int maDG, int pageNumber, int pageSize);
        Task<IEnumerable<PhanHoiDanhGia>> GetByMaKHAsync(int maKH);
        Task AddAsync(PhanHoiDanhGia phanHoiDanhGia);
        Task UpdateAsync(PhanHoiDanhGia phanHoiDanhGia);
        Task DeleteAsync(int maPHDG);
    }
}
