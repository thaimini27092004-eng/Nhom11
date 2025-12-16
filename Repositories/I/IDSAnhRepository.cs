using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I
{
    public interface IDSAnhRepository
    {
        Task AddAsync(DSAnh dsAnh);
        Task DeleteByMaSPAsync(int maSP); // Thêm phương thức xóa ảnh theo ProductId
        Task DeleteAsync(int id); // Thêm phương thức xóa ảnh theo ID
    }
}
