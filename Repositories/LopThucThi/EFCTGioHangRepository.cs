using WebsiteBanHang.Models.GioHang;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories
{
    public class EFCTGioHangRepository : ICTGioHangRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCTGioHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CTGioHang> GetCartItemAsync(int maGH, int productId)
        {
            // Tìm sản phẩm trong giỏ dựa trên MaGH và Id
            return await _context.CTGioHang
                .FirstOrDefaultAsync(ct => ct.MaGH == maGH && ct.MaSP == productId);
        }

        public async Task AddCartItemAsync(CTGioHang item)
        {
            // Thêm sản phẩm mới vào giỏ
            _context.CTGioHang.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CTGioHang item)
        {
            // Cập nhật thông tin sản phẩm (số lượng, giá, thời gian)
            _context.CTGioHang.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartItemAsync(int maGH, int productId)
        {
            // Xóa sản phẩm khỏi giỏ
            var item = await _context.CTGioHang
                .FirstOrDefaultAsync(ct => ct.MaGH == maGH && ct.MaSP == productId);
            if (item != null)
            {
                _context.CTGioHang.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CTGioHang>> GetCartItemsAsync(int maGH)
        {
            // Lấy danh sách sản phẩm trong giỏ, bao gồm thông tin Product
            return await _context.CTGioHang
                .Include(ct => ct.SanPham) // Đảm bảo lấy được Name từ Product
                .Where(ct => ct.MaGH == maGH)
                .ToListAsync();
        }

        public async Task ClearCartItemsAsync(int maGH)
        {
            // Xóa toàn bộ sản phẩm trong giỏ
            var items = await _context.CTGioHang
                .Where(ct => ct.MaGH == maGH)
                .ToListAsync();
            if (items.Any())
            {
                _context.CTGioHang.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}