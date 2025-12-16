using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFLikeDanhGiaRepository : ILikeDanhGiaRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLikeDanhGiaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LikeDanhGia>> GetAllAsync()
        {
            return await _context.LikeDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.DanhGia)
                .ToListAsync();
        }

        public async Task<LikeDanhGia> GetByIdAsync(int maKH, int maDG)
        {
            var like = await _context.LikeDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.DanhGia)
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaDG == maDG);

            if (like == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaDG={maDG}.");
            }

            return like;
        }

        public async Task<IEnumerable<LikeDanhGia>> GetByMaDGAsync(int maDG)
        {
            return await _context.LikeDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.DanhGia)
                .Where(l => l.MaDG == maDG)
                .ToListAsync();
        }

        public async Task<IEnumerable<LikeDanhGia>> GetByMaKHAsync(int maKH)
        {
            return await _context.LikeDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.DanhGia)
                .Where(l => l.MaKH == maKH)
                .ToListAsync();
        }

        public async Task AddAsync(LikeDanhGia likeDanhGia)
        {
            if (likeDanhGia == null)
            {
                throw new ArgumentNullException(nameof(likeDanhGia));
            }

            if (await ExistsAsync(likeDanhGia.MaKH, likeDanhGia.MaDG))
            {
                throw new InvalidOperationException($"Khách hàng MaKH={likeDanhGia.MaKH} đã like đánh giá MaDG={likeDanhGia.MaDG}.");
            }

            _context.LikeDanhGia.Add(likeDanhGia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maKH, int maDG)
        {
            var like = await _context.LikeDanhGia
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaDG == maDG);

            if (like != null)
            {
                _context.LikeDanhGia.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaDG={maDG}.");
            }
        }

        public async Task<bool> ExistsAsync(int maKH, int maDG)
        {
            return await _context.LikeDanhGia
                .AnyAsync(l => l.MaKH == maKH && l.MaDG == maDG);
        }
    }
}