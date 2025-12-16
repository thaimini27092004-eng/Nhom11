using WebsiteBanHang.Models.GioHang;
using System.Threading.Tasks;

namespace WebsiteBanHang.Repositories.I
{
    public interface IGioHangRepository
    {
        // Lấy giỏ hàng theo MaKH, tạo mới nếu chưa tồn tại
        Task<GioHang> GetOrCreateGioHangAsync(int maKH);


        // Lấy giỏ hàng theo MaGH (khóa chính)
        //Task<LuuGioHang> GetGioHangByIdAsync(int maGH);
    }
}