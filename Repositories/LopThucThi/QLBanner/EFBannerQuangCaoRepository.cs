using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QuanLyBanner;
using WebsiteBanHang.Repositories.I.QLBanner;

namespace WebsiteBanHang.Repositories.LopThucThi.QLBanner
{
    public class EFBannerQuangCaoRepository : IBannerQuangCaoRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBannerQuangCaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BannerQuangCao>> GetAllAsync()
        {
            return await _context.BannerQuangCao.ToListAsync();
        }

        public async Task<BannerQuangCao> GetByIdAsync(int id)
        {
            return await _context.BannerQuangCao.FirstOrDefaultAsync(b => b.MaAQC == id);
        }

        public async Task AddAsync(BannerQuangCao banner)
        {
            _context.BannerQuangCao.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BannerQuangCao banner)
        {
            _context.BannerQuangCao.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.BannerQuangCao.FindAsync(id);
            if (banner != null)
            {
                _context.BannerQuangCao.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }
    }
}