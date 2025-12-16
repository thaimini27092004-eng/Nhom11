using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface IPhanHoiBinhLuanRepository
    {
        // Lấy câu trả lời theo MaPH và MaKH
        Task<PhanHoiBinhLuan> GetByIdAsync(int maTL);

        // Lấy danh sách câu trả lời theo MaPH
        Task<IEnumerable<PhanHoiBinhLuan>> GetByMaPHAsync(int maPH);

        // Thêm câu trả lời mới
        Task AddAsync(PhanHoiBinhLuan traLoi);

        // Cập nhật câu trả lời
        Task UpdateAsync(PhanHoiBinhLuan traLoi);

        // Xóa câu trả lời theo MaPH và MaKH
        Task DeleteAsync(int maTL);
        Task<IEnumerable<PhanHoiBinhLuan>> GetByMaPHPagedAsync(int maPH, int pageNumber, int pageSize);
    }
}
