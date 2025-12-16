using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFPhanHoiDanhGiaRepository : IPhanHoiDanhGiaRepository
    {
        private readonly ApplicationDbContext _context;

        public EFPhanHoiDanhGiaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhanHoiDanhGia>> GetAllAsync()
        {
            return await _context.PhanHoiDanhGia
                .Include(ph => ph.KhachHang)
                .Include(ph => ph.DanhGia)
                .ToListAsync();
        }

        public async Task<PhanHoiDanhGia> GetByIdAsync(int maPHDG)
        {
            var phanHoi = await _context.PhanHoiDanhGia
                .Include(ph => ph.KhachHang)
                .Include(ph => ph.DanhGia)
                .FirstOrDefaultAsync(ph => ph.MaPHDG == maPHDG);

            if (phanHoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPHDG={maPHDG}.");
            }

            return phanHoi;
        }

        public async Task<IEnumerable<PhanHoiDanhGia>> GetByMaDGPagedAsync(int maDG, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            return await _context.PhanHoiDanhGia
                .Include(ph => ph.KhachHang)
                .Where(ph => ph.MaDG == maDG)
                .GroupJoin(_context.LikePhanHoiDanhGia,
                    ph => ph.MaPHDG,
                    lph => lph.MaPHDG,
                    (ph, lphs) => new { PhanHoiDanhGia = ph, SoLuotThich = lphs.Count() })
                .OrderByDescending(x => x.SoLuotThich) // Sắp xếp theo lượt thích giảm dần
                .ThenByDescending(x => x.PhanHoiDanhGia.NgayPHDG) // Nếu cùng lượt thích, sắp xếp theo ngày giảm dần
                .Skip(skip)
                .Take(pageSize)
                .Select(x => x.PhanHoiDanhGia)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhanHoiDanhGia>> GetByMaKHAsync(int maKH)
        {
            return await _context.PhanHoiDanhGia
                .Include(ph => ph.KhachHang)
                .Include(ph => ph.DanhGia)
                .Where(ph => ph.MaKH == maKH)
                .ToListAsync();
        }

        public async Task AddAsync(PhanHoiDanhGia phanHoiDanhGia)
        {
            if (phanHoiDanhGia == null)
            {
                throw new ArgumentNullException(nameof(phanHoiDanhGia));
            }

            _context.PhanHoiDanhGia.Add(phanHoiDanhGia);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhanHoiDanhGia phanHoiDanhGia)
        {
            if (phanHoiDanhGia == null)
            {
                throw new ArgumentNullException(nameof(phanHoiDanhGia));
            }

            var existingPhanHoi = await _context.PhanHoiDanhGia
                .FirstOrDefaultAsync(ph => ph.MaPHDG == phanHoiDanhGia.MaPHDG);
            if (existingPhanHoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPHDG={phanHoiDanhGia.MaPHDG}.");
            }

            _context.Entry(existingPhanHoi).CurrentValues.SetValues(phanHoiDanhGia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maPHDG)
        {
            var phanHoi = await _context.PhanHoiDanhGia
                .FirstOrDefaultAsync(ph => ph.MaPHDG == maPHDG);
            if (phanHoi != null)
            {
                _context.PhanHoiDanhGia.Remove(phanHoi);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPHDG={maPHDG}.");
            }
        }
    }
}