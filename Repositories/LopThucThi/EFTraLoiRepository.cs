using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.LopThucThi
{
    public class EFTraLoiRepository : IPhanHoiBinhLuanRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTraLoiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Lấy tất cả câu trả lời (phan hoi + khachhang)
        public async Task<IEnumerable<PhanHoiBinhLuan>> GetAllAsync()
        {
            return await _context.Set<PhanHoiBinhLuan>()
                .Include(tl => tl.BinhLuan)
                .Include(tl => tl.KhachHang)
                .ToListAsync();
        }

        //Lấy một câu trả lời theo khóa chính (MaTL)
        public async Task<PhanHoiBinhLuan> GetByIdAsync(int maTL)
        {
            var traLoi = await _context.Set<PhanHoiBinhLuan>()
                .Include(tl => tl.BinhLuan)
                .Include(tl => tl.KhachHang)
                .FirstOrDefaultAsync(tl => tl.MaPHBL == maTL);
            if (traLoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy câu trả lời với MaTL={maTL}.");
            }

            return traLoi;
        }

        // Lấy danh sách câu trả lời theo MaPH
        public async Task<IEnumerable<PhanHoiBinhLuan>> GetByMaPHAsync(int maPH)
        {
            try
            {
                return await _context.Set<PhanHoiBinhLuan>()
                    .Where(tl => tl.MaBL == maPH)
                    .Select(tl => new PhanHoiBinhLuan
                    {
                        MaPHBL = tl.MaPHBL,
                        MaBL = tl.MaBL,
                        MaKH = tl.MaKH,
                        NoiDungPHBL = tl.NoiDungPHBL,
                        NgayPHBL = tl.NgayPHBL,
                        KhachHang = new KhachHang
                        {
                            TenKH = tl.KhachHang.TenKH,
                            AvatarUrl = tl.KhachHang.AvatarUrl // Thêm AvatarUrl
                        }
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách câu trả lời cho MaPH={maPH}", ex);
            }
        }


        // Lấy danh sách câu trả lời theo MaKH
        public async Task<IEnumerable<PhanHoiBinhLuan>> GetByMaSPAsync(int maKH)
        {
            try
            {
                return await _context.Set<PhanHoiBinhLuan>()
                    .Include(tl => tl.BinhLuan)
                    .Include(tl => tl.KhachHang)
                    .Where(tl => tl.MaKH == maKH)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách câu trả lời cho MaKH={maKH}", ex);
            }
        }

        //thêm câu trả lời
        public async Task AddAsync(PhanHoiBinhLuan traLoi)
        {
            if (traLoi == null)
            {
                throw new ArgumentNullException(nameof(traLoi));
            }

            _context.Set<PhanHoiBinhLuan>().Add(traLoi);
            await _context.SaveChangesAsync();
        }

        // Sửa câu trả lời
        public async Task UpdateAsync(PhanHoiBinhLuan traLoi)
        {
            if (traLoi == null)
            {
                throw new ArgumentNullException(nameof(traLoi));
            }

            var existingTraLoi = await _context.Set<PhanHoiBinhLuan>()
                .FirstOrDefaultAsync(tl => tl.MaPHBL == traLoi.MaPHBL);
            if (existingTraLoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy câu trả lời với MaTL={traLoi.MaPHBL}.");
            }

            _context.Entry(existingTraLoi).CurrentValues.SetValues(traLoi);
            await _context.SaveChangesAsync();
        }

        // Xóa câu trả lời
        public async Task DeleteAsync(int maTL)
        {
            var traLoi = await _context.Set<PhanHoiBinhLuan>()
                .FirstOrDefaultAsync(tl => tl.MaPHBL == maTL);
            if (traLoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy câu trả lời với MaTL={maTL}.");
            }

            _context.Set<PhanHoiBinhLuan>().Remove(traLoi);
            await _context.SaveChangesAsync();
        }

        //Phân Trang
        public async Task<IEnumerable<PhanHoiBinhLuan>> GetByMaPHPagedAsync(int maPH, int pageNumber, int pageSize)
        {
            try
            {
                int skip = (pageNumber - 1) * pageSize;
                return await _context.Set<PhanHoiBinhLuan>()
                    .Where(tl => tl.MaBL == maPH)
                    .Select(tl => new PhanHoiBinhLuan
                    {
                        MaPHBL = tl.MaPHBL,
                        MaBL = tl.MaBL,
                        MaKH = tl.MaKH,
                        NoiDungPHBL = tl.NoiDungPHBL,
                        NgayPHBL = tl.NgayPHBL,
                        KhachHang = new KhachHang
                        {
                            TenKH = tl.KhachHang.TenKH,
                            AvatarUrl = tl.KhachHang.AvatarUrl
                        }
                    })
                    .OrderByDescending(tl => tl.NgayPHBL)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách trả lời phân trang cho MaPH={maPH}", ex);
            }
        }
    }
}
