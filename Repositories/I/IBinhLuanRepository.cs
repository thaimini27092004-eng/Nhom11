using WebsiteBanHang.Models.QLDanhGia_PhanHoi;

namespace WebsiteBanHang.Repositories.I
{
    public interface IBinhLuanRepository
    {
        // Lấy tất cả phản hồi
        Task<IEnumerable<BinhLuan>> GetAllAsync();

        // Lấy phản hồi theo MaPH
        Task<BinhLuan> GetByIdAsync(int maPH);

        //Phân Trang
        Task<IEnumerable<BinhLuan>> GetByMaSPPagedAsync(int maSP, int pageNumber, int pageSize);

        // Lấy danh sách phản hồi theo MaKH
        Task<IEnumerable<BinhLuan>> GetByMaKHAsync(int maKH);

        // Lấy danh sách phản hồi theo MaDG
        Task<IEnumerable<BinhLuan>> GetByMaDGAsync(int maDG);

        Task<IEnumerable<BinhLuan>> GetByMaSPAsync(int maSP);

        // Thêm phản hồi mới
        Task AddAsync(BinhLuan phanHoi);

        // Cập nhật phản hồi
        Task UpdateAsync(BinhLuan phanHoi);

        // Xóa phản hồi theo MaPH
        Task DeleteAsync(int maPH);
    }
}
