using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface IAnhDGRepository
    {
        // Lấy tất cả ảnh đánh giá
        Task<IEnumerable<AnhDG>> GetAllAsync();

        // Lấy ảnh đánh giá theo MaAnhDG
        Task<AnhDG> GetByIdAsync(int maAnhDG);

        // Lấy danh sách ảnh đánh giá theo MaDG
        Task<IEnumerable<AnhDG>> GetByMaDGAsync(int maDG);

        // Thêm ảnh đánh giá mới
        Task AddAsync(AnhDG anhDG);

        // Cập nhật ảnh đánh giá
        Task UpdateAsync(AnhDG anhDG);

        // Xóa ảnh đánh giá theo MaAnhDG
        Task DeleteAsync(int maAnhDG);
    }
}
