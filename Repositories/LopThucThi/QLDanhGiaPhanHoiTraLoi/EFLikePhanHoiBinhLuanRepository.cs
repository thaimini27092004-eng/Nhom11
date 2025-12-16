using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFLikePhanHoiBinhLuanRepository : ILikePhanHoiBinhLuanRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLikePhanHoiBinhLuanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả lượt like trả lời
        public async Task<IEnumerable<LikePhanHoiBinhLuan>> GetAllAsync()
        {
            return await _context.LikePhanHoiBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.TraLoi)
                .ToListAsync();
        }

        // Lấy lượt like theo MaKH và MaTL
        public async Task<LikePhanHoiBinhLuan> GetByIdAsync(int maKH, int maTL)
        {
            var like = await _context.LikePhanHoiBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.TraLoi)
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaPHBL == maTL);

            if (like == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaTL={maTL}.");
            }

            return like;
        }

        // Lấy danh sách lượt like theo MaTL
        public async Task<IEnumerable<LikePhanHoiBinhLuan>> GetByMaTLAsync(int maTL)
        {
            return await _context.LikePhanHoiBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.TraLoi)
                .Where(l => l.MaPHBL == maTL)
                .ToListAsync();
        }

        // Lấy danh sách lượt like theo MaKH
        public async Task<IEnumerable<LikePhanHoiBinhLuan>> GetByMaKHAsync(int maKH)
        {
            return await _context.LikePhanHoiBinhLuan
                .Include(l => l.KhachHang)
                .Include(l => l.TraLoi)
                .Where(l => l.MaKH == maKH)
                .ToListAsync();
        }

        // Thêm lượt like mới
        public async Task AddAsync(LikePhanHoiBinhLuan likeTraLoi)
        {
            if (likeTraLoi == null)
            {
                throw new ArgumentNullException(nameof(likeTraLoi));
            }

            if (await ExistsAsync(likeTraLoi.MaKH, likeTraLoi.MaPHBL))
            {
                throw new InvalidOperationException($"Khách hàng MaKH={likeTraLoi.MaKH} đã like trả lời MaTL={likeTraLoi.MaPHBL}.");
            }

            _context.LikePhanHoiBinhLuan.Add(likeTraLoi);
            await _context.SaveChangesAsync();
        }

        // Xóa lượt like
        public async Task DeleteAsync(int maKH, int maTL)
        {
            var like = await _context.LikePhanHoiBinhLuan
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaPHBL == maTL);

            if (like != null)
            {
                _context.LikePhanHoiBinhLuan.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaTL={maTL}.");
            }
        }

        // Kiểm tra xem khách hàng đã like trả lời chưa
        public async Task<bool> ExistsAsync(int maKH, int maTL)
        {
            return await _context.LikePhanHoiBinhLuan
                .AnyAsync(l => l.MaKH == maKH && l.MaPHBL == maTL);
        }
    }
}