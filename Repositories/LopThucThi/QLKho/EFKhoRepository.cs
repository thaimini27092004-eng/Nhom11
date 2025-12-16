using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QLTonKho;
using WebsiteBanHang.Repositories.I.QLKhoHang;

namespace WebsiteBanHang.Repositories.LopThucThi.QLKho
{
    public class EFKhoRepository : IKhoRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Kho>> GetAllAsync()
        {
            return await _context.Kho.ToListAsync();
        }

        public async Task<Kho> GetByIdAsync(int maKho)
        {
            var kho = await _context.Kho.FindAsync(maKho);
            if (kho == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {maKho} not found.");
            }
            return kho;
        }

        public async Task AddAsync(Kho kho)
        {
            if (kho == null)
            {
                throw new ArgumentNullException(nameof(kho));
            }

            _context.Kho.Add(kho);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Kho kho)
        {
            _context.Kho.Update(kho);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maKho)
        {
            var kho = await _context.Kho.FindAsync(maKho);
            if (kho != null)
            {
                _context.Kho.Remove(kho);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Kho>> GetDisplayedAsync()
        {
            return await _context.Kho
                .Where(k => k.TTHienThi)
                .ToListAsync();
        }
    }
}