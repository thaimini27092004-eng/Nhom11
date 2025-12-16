using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFPTTTRepository : IPTTTRepository
    {
        private readonly ApplicationDbContext _context;

        public EFPTTTRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PTTT>> GetAllAsync()
        {
            return await _context.PTTT.ToListAsync();
        }

        public async Task<PTTT> GetByIdAsync(int maPT)
        {
            var pttt = await _context.PTTT.FindAsync(maPT);
            if (pttt == null)
            {
                throw new KeyNotFoundException($"Payment Method with ID {maPT} not found.");
            }
            return pttt;
        }

        public async Task AddAsync(PTTT pttt)
        {
            if (pttt == null)
            {
                throw new ArgumentNullException(nameof(pttt));
            }

            _context.PTTT.Add(pttt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PTTT pttt)
        {
            _context.PTTT.Update(pttt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maPT)
        {
            var pttt = await _context.PTTT.FindAsync(maPT);
            if (pttt != null)
            {
                _context.PTTT.Remove(pttt);
                await _context.SaveChangesAsync();
            }
        }
    }
}