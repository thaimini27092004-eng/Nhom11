using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFDSAnhRepository : IDSAnhRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDSAnhRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DSAnh dsAnh)
        {
            await _context.DSAnh.AddAsync(dsAnh);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByMaSPAsync(int maSP)
        {
            var anhs = await _context.DSAnh
                .Where(img => img.MaSP == maSP)
                .ToListAsync();

            if (anhs != null && anhs.Any())
            {
                _context.DSAnh.RemoveRange(anhs);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var dsAnh = await _context.DSAnh.FindAsync(id);
            if (dsAnh != null)
            {
                _context.DSAnh.Remove(dsAnh);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully deleted image with ID: {id}");
            }
            else
            {
                Console.WriteLine($"Image with ID {id} not found in database");
            }
        }

    }
}
