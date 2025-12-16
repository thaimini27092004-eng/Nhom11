using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFLikeBinhLuanRepository : ILikeBinhLuanRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLikeBinhLuanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả lượt like phản hồi
        public async Task<IEnumerable<LikeBinhLuan>> GetAllAsync()
        {
            return await _context.LikeBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.BinhLuan)
                .ToListAsync();
        }

        // Lấy lượt like theo MaKH và MaPH
        public async Task<LikeBinhLuan> GetByIdAsync(int maKH, int maPH)
        {
            var like = await _context.LikeBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.BinhLuan)
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaBL == maPH);

            if (like == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaPH={maPH}.");
            }

            return like;
        }

        // Lấy danh sách lượt like theo MaPH
        public async Task<IEnumerable<LikeBinhLuan>> GetByMaPHAsync(int maPH)
        {
            return await _context.LikeBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.BinhLuan)
                .Where(l => l.MaBL == maPH)
                .ToListAsync();
        }

        // Lấy danh sách lượt like theo MaKH
        public async Task<IEnumerable<LikeBinhLuan>> GetByMaKHAsync(int maKH)
        {
            return await _context.LikeBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.BinhLuan)
                .Where(l => l.MaKH == maKH)
                .ToListAsync();
        }

        // Thêm lượt like mới
        public async Task AddAsync(LikeBinhLuan likePhanHoi)
        {
            if (likePhanHoi == null)
            {
                throw new ArgumentNullException(nameof(likePhanHoi));
            }

            if (await ExistsAsync(likePhanHoi.MaKH, likePhanHoi.MaBL))
            {
                throw new InvalidOperationException($"Khách hàng MaKH={likePhanHoi.MaKH} đã like phản hồi MaPH={likePhanHoi.MaBL}.");
            }

            _context.LikeBinhLuan.Add(likePhanHoi);
            await _context.SaveChangesAsync();
        }

        // Xóa lượt like
        public async Task DeleteAsync(int maKH, int maPH)
        {
            var like = await _context.LikeBinhLuan
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaBL == maPH);

            if (like != null)
            {
                _context.LikeBinhLuan.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaPH={maPH}.");
            }
        }

        // Kiểm tra xem khách hàng đã like phản hồi chưa
        public async Task<bool> ExistsAsync(int maKH, int maPH)
        {
            return await _context.LikeBinhLuan
                .AnyAsync(l => l.MaKH == maKH && l.MaBL == maPH);
        }
    }
}