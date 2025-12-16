using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi;

namespace WebsiteBanHang.Repositories.LopThucThi.QLDanhGiaPhanHoiTraLoi
{
    public class EFLikePhanHoiDanhGiaRepository : ILikePhanHoiDanhGiaRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLikePhanHoiDanhGiaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LikePhanHoiDanhGia>> GetAllAsync()
        {
            return await _context.LikePhanHoiDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.PhanHoiDanhGia)
                .ToListAsync();
        }

        public async Task<LikePhanHoiDanhGia> GetByIdAsync(int maKH, int maPHDG)
        {
            var like = await _context.LikePhanHoiDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.PhanHoiDanhGia)
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaPHDG == maPHDG);

            if (like == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaPHDG={maPHDG}.");
            }

            return like;
        }

        public async Task<IEnumerable<LikePhanHoiDanhGia>> GetByMaPHDGAsync(int maPHDG)
        {
            return await _context.LikePhanHoiDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.PhanHoiDanhGia)
                .Where(l => l.MaPHDG == maPHDG)
                .ToListAsync();
        }

        public async Task<IEnumerable<LikePhanHoiDanhGia>> GetByMaKHAsync(int maKH)
        {
            return await _context.LikePhanHoiDanhGia
                .Include(l => l.KhachHang)
                .Include(l => l.PhanHoiDanhGia)
                .Where(l => l.MaKH == maKH)
                .ToListAsync();
        }

        public async Task AddAsync(LikePhanHoiDanhGia likePhanHoiDanhGia)
        {
            if (likePhanHoiDanhGia == null)
            {
                throw new ArgumentNullException(nameof(likePhanHoiDanhGia));
            }

            if (await ExistsAsync(likePhanHoiDanhGia.MaKH, likePhanHoiDanhGia.MaPHDG))
            {
                throw new InvalidOperationException($"Khách hàng MaKH={likePhanHoiDanhGia.MaKH} đã like phản hồi MaPHDG={likePhanHoiDanhGia.MaPHDG}.");
            }

            _context.LikePhanHoiDanhGia.Add(likePhanHoiDanhGia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maKH, int maPHDG)
        {
            var like = await _context.LikePhanHoiDanhGia
                .FirstOrDefaultAsync(l => l.MaKH == maKH && l.MaPHDG == maPHDG);

            if (like != null)
            {
                _context.LikePhanHoiDanhGia.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy lượt like với MaKH={maKH} và MaPHDG={maPHDG}.");
            }
        }

        public async Task<bool> ExistsAsync(int maKH, int maPHDG)
        {
            return await _context.LikePhanHoiDanhGia
                .AnyAsync(l => l.MaKH == maKH && l.MaPHDG == maPHDG);
        }
    }
}
