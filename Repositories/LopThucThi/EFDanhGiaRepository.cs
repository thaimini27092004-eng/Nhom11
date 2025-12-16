using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFDanhGiaRepository : IDanhGiaRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDanhGiaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DanhGia>> GetAllAsync()
        {
            return await _context.DanhGia
                .Include(dg => dg.CTHD)
                .ThenInclude(ct => ct.HoaDon)
                .ThenInclude(hd => hd.KhachHang)
                .ToListAsync();
        }

        public async Task<DanhGia> GetByIdAsync(int maDG)
        {
            var danhGia = await _context.DanhGia
                .Include(dg => dg.CTHD)
                .ThenInclude(ct => ct.HoaDon)
                .ThenInclude(hd => hd.KhachHang)
                .FirstOrDefaultAsync(dg => dg.MaDG == maDG);

            if (danhGia == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với MaDG={maDG}.");
            }

            return danhGia;
        }

        public async Task<IEnumerable<DanhGia>> GetByMaSPPagedAsync(int maSP, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            return await _context.DanhGia
                .Include(dg => dg.CTHD)
                .ThenInclude(ct => ct.HoaDon)
                .ThenInclude(hd => hd.KhachHang)
                .Where(dg => dg.MaSP == maSP && dg.TTHienThi == true)
                .GroupJoin(_context.LikeDanhGia,
                    dg => dg.MaDG,
                    ldg => ldg.MaDG,
                    (dg, ldgs) => new { DanhGia = dg, SoLuotThich = ldgs.Count() })
                .OrderByDescending(x => x.DanhGia.Sao) // Sắp xếp theo số sao giảm dần
                .ThenByDescending(x => x.SoLuotThich) // Nếu cùng số sao, sắp xếp theo lượt thích giảm dần
                .ThenByDescending(x => x.DanhGia.NgayDG) // Nếu cùng lượt thích, sắp xếp theo ngày giảm dần
                .Skip(skip)
                .Take(pageSize)
                .Select(x => x.DanhGia)
                .ToListAsync();


           

        }

        public async Task<IEnumerable<DanhGia>> GetByMaKHAsync(int maKH)
        {
            return await _context.DanhGia
                .Include(dg => dg.CTHD)
                .ThenInclude(ct => ct.HoaDon)
                .ThenInclude(hd => hd.KhachHang)
                .Where(dg => dg.CTHD.HoaDon.MaKH == maKH)
                .ToListAsync();
        }

        public async Task AddAsync(DanhGia danhGia)
        {
            if (danhGia == null)
            {
                throw new ArgumentNullException(nameof(danhGia));
            }

            _context.DanhGia.Add(danhGia);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DanhGia danhGia)
        {
            if (danhGia == null)
            {
                throw new ArgumentNullException(nameof(danhGia));
            }

            var existingDanhGia = await _context.DanhGia
                .FirstOrDefaultAsync(dg => dg.MaDG == danhGia.MaDG);
            if (existingDanhGia == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với MaDG={danhGia.MaDG}.");
            }

            _context.Entry(existingDanhGia).CurrentValues.SetValues(danhGia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maDG)
        {
            var danhGia = await _context.DanhGia
                .FirstOrDefaultAsync(dg => dg.MaDG == maDG);
            if (danhGia != null)
            {
                _context.DanhGia.Remove(danhGia);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với MaDG={maDG}.");
            }
        }

        public async Task<bool> ExistsAsync(int soHD, int maSP, int maKho)
        {
            return await _context.DanhGia
                .AnyAsync(dg => dg.SoHD == soHD && dg.MaSP == maSP && dg.MaKho == maKho);
        }
    }
}