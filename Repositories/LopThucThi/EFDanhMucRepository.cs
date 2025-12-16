using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFDanhMucRepository : IDanhMucRepository
    {
        private readonly ApplicationDbContext _context;
        public EFDanhMucRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Tương tự như EFProductRepository, nhưng cho Category
        public async Task<IEnumerable<DanhMuc>> GetAllAsync()
        {
            return await _context.DanhMuc.ToListAsync();
        }

        public async Task<DanhMuc> GetByIdAsync(int maDM)
        {
            var danhmuc = await _context.DanhMuc.FindAsync(maDM);
            if (danhmuc == null)
            {
                throw new KeyNotFoundException($"Category with ID {maDM} not found.");
            }
            return danhmuc;
        }

        public async Task AddAsync(DanhMuc danhmuc)
        {
            if (danhmuc == null)
            {
                throw new ArgumentNullException(nameof(danhmuc));
            }

            _context.DanhMuc.Add(danhmuc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DanhMuc danhmuc)
        {
            _context.DanhMuc.Update(danhmuc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maDM)
        {
            var danhmuc = await _context.DanhMuc.FindAsync(maDM);
            if (danhmuc != null)
            {
                _context.DanhMuc.Remove(danhmuc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DanhMuc>> GetDisplayedAsync()
        {
            return await _context.DanhMuc
                .Where(d => d.TTHienThi)
                .ToListAsync();
        }
    }
}
