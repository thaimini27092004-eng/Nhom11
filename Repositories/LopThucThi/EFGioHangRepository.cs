using WebsiteBanHang.Models.GioHang;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using System.Threading.Tasks;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories
{
    public class EFGioHangRepository : IGioHangRepository
    {
        private readonly ApplicationDbContext _context;

        public EFGioHangRepository(ApplicationDbContext context)
        {
            _context = context; // Tiêm DbContext để truy cập dữ liệu
        }

        public async Task<GioHang> GetOrCreateGioHangAsync(int maKH)
        {
            // Tìm giỏ hàng dựa trên MaKH
            var gioHang = await _context.GioHang
                .FirstOrDefaultAsync(gh => gh.MaKH == maKH);
            if (gioHang == null)
            {
                // Nếu không có, tạo mới giỏ hàng
                gioHang = new GioHang { MaKH = maKH };
                _context.GioHang.Add(gioHang);
                await _context.SaveChangesAsync(); // Lưu để sinh MaGH
            }
            return gioHang;
        }
    }
}