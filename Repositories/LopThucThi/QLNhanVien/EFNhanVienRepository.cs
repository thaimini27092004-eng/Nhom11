using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I.QLNhanVien;

namespace WebsiteBanHang.Repositories.LopThucThi.QLNhanVien
{
    public class EFNhanVienRepository : INhanVienRepository
    {
        private readonly ApplicationDbContext _context;

        public EFNhanVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NhanVien> GetByUserIdAsync(string userId)
        {
            return await _context.NhanVien
                .Include(nv => nv.User)
                .FirstOrDefaultAsync(nv => nv.UserId == userId);
        }

        public async Task AddAsync(NhanVien nhanVien)
        {
            if (nhanVien == null)
            {
                throw new ArgumentNullException(nameof(nhanVien));
            }

            _context.NhanVien.Add(nhanVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maNV)
        {
            var nhanVien = await _context.NhanVien.FindAsync(maNV);
            if (nhanVien != null)
            {
                _context.NhanVien.Remove(nhanVien);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.NhanVien.AnyAsync(nv => nv.UserId == userId);
        }
    }

}
