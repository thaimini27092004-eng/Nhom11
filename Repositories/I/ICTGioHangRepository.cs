using WebsiteBanHang.Models.GioHang;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebsiteBanHang.Repositories.I
{
    public interface ICTGioHangRepository
    {
        // Lấy sản phẩm trong giỏ theo MaGH và ProductId
        Task<CTGioHang> GetCartItemAsync(int maGH, int productId);

        
        // Thêm sản phẩm mới vào giỏ
        Task AddCartItemAsync(CTGioHang item);

        
        // Cập nhật sản phẩm trong giỏ
        Task UpdateCartItemAsync(CTGioHang item);

        
        // Xóa sản phẩm khỏi giỏ
        Task DeleteCartItemAsync(int maGH, int productId);

        
        // Lấy tất cả sản phẩm trong giỏ, bao gồm thông tin Product
        Task<List<CTGioHang>> GetCartItemsAsync(int maGH);

       
        // Xóa toàn bộ sản phẩm trong giỏ
        Task ClearCartItemsAsync(int maGH);
    }
}