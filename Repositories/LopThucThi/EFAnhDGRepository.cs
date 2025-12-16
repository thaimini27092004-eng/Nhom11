using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFAnhDGRepository : IAnhDGRepository
    {
        private readonly ApplicationDbContext _context;

        public EFAnhDGRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnhDG>> GetAllAsync()
        {
            return await _context.Set<AnhDG>()
                .Include(adg => adg.DanhGia) // Include thông tin DanhGia nếu cần
                .ToListAsync();
        }

        public async Task<AnhDG> GetByIdAsync(int maAnhDG)
        {
            var anhDG = await _context.Set<AnhDG>()
                .Include(adg => adg.DanhGia)
                .FirstOrDefaultAsync(adg => adg.MaAnhDG == maAnhDG);

            if (anhDG == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy ảnh đánh giá với MaAnhDG={maAnhDG}.");
            }

            return anhDG;
        }

        public async Task<IEnumerable<AnhDG>> GetByMaDGAsync(int maDG)
        {
            return await _context.Set<AnhDG>()
                .Include(adg => adg.DanhGia)
                .Where(adg => adg.MaDG == maDG)
                .ToListAsync();
        }

        public async Task AddAsync(AnhDG anhDG)
        {
            if (anhDG == null)
            {
                throw new ArgumentNullException(nameof(anhDG));
            }

            _context.Set<AnhDG>().Add(anhDG);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AnhDG anhDG)
        {
            if (anhDG == null)
            {
                throw new ArgumentNullException(nameof(anhDG));
            }

            var existingAnhDG = await _context.Set<AnhDG>()
                .FirstOrDefaultAsync(adg => adg.MaAnhDG == anhDG.MaAnhDG);
            if (existingAnhDG == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy ảnh đánh giá với MaAnhDG={anhDG.MaAnhDG}.");
            }

            _context.Entry(existingAnhDG).CurrentValues.SetValues(anhDG);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maAnhDG)
        {
            var anhDG = await _context.Set<AnhDG>()
                .FirstOrDefaultAsync(adg => adg.MaAnhDG == maAnhDG);
            if (anhDG != null)
            {
                _context.Set<AnhDG>().Remove(anhDG);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy ảnh đánh giá với MaAnhDG={maAnhDG}.");
            }
        }
    }
}