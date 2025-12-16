using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFTrangThaiRepository : ITrangThaiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTrangThaiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrangThai>> GetAllAsync()
        {
            return await _context.TrangThai.ToListAsync();
        }

        public async Task<TrangThai> GetByIdAsync(int maTT)
        {
            var trangThai = await _context.TrangThai.FindAsync(maTT);
            if (trangThai == null)
            {
                throw new KeyNotFoundException($"Status with ID {maTT} not found.");
            }
            return trangThai;
        }

        public async Task AddAsync(TrangThai trangThai)
        {
            if (trangThai == null)
            {
                throw new ArgumentNullException(nameof(trangThai));
            }

            _context.TrangThai.Add(trangThai);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TrangThai trangThai)
        {
            _context.TrangThai.Update(trangThai);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maTT)
        {
            var trangThai = await _context.TrangThai.FindAsync(maTT);
            if (trangThai != null)
            {
                _context.TrangThai.Remove(trangThai);
                await _context.SaveChangesAsync();
            }
        }
    }
}