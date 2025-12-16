using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.LopThucThi
{
    public class EFCTHDRepository : ICTHDRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EFCTHDRepository> _logger; // Thêm ILogger

        // Constructor nhận ApplicationDbContext để tương tác với cơ sở dữ liệu
        public EFCTHDRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả chi tiết hóa đơn
        public async Task<IEnumerable<CTHD>> GetAllAsync()
        {
            return await _context.CTHD
                .Include(ct => ct.HoaDon) // Bao gồm thông tin hóa đơn
                .Include(ct => ct.TonKho) // Bao gồm thông tin TonKho
                .ThenInclude(tk => tk.SanPham) // Bao gồm thông tin sản phẩm qua TonKho
                .Include(ct => ct.TonKho) // Bao gồm thông tin TonKho
                .ThenInclude(tk => tk.Kho) // Bao gồm thông tin kho qua TonKho
                .ToListAsync();
        }

        // Lấy chi tiết hóa đơn theo khóa chính composite
        public async Task<CTHD> GetByIdAsync(int soHD, int maSP, int maKho)
        {
            var cthd = await _context.CTHD
                .Include(ct => ct.HoaDon)
                .Include(ct => ct.TonKho)
                .ThenInclude(tk => tk.SanPham)
                .Include(ct => ct.TonKho)
                .ThenInclude(tk => tk.Kho)
                .FirstOrDefaultAsync(ct => ct.SoHD == soHD && ct.MaSP == maSP && ct.MaKho == maKho);

            if (cthd == null)
            {
                throw new KeyNotFoundException($"Chi tiết hóa đơn với SoHD={soHD}, MaSP={maSP}, MaKho={maKho} không tồn tại.");
            }

            return cthd;
        }

        // Thêm chi tiết hóa đơn mới
        public async Task AddAsync(CTHD cthd)
        {
            if (cthd == null)
            {
                throw new ArgumentNullException(nameof(cthd));
            }

            _context.CTHD.Add(cthd);
            await _context.SaveChangesAsync();
        }

        // Cập nhật chi tiết hóa đơn
        public async Task UpdateAsync(CTHD cthd)
        {
            if (cthd == null)
            {
                throw new ArgumentNullException(nameof(cthd));
            }

            _context.CTHD.Update(cthd);
            await _context.SaveChangesAsync();
        }

        // Xóa chi tiết hóa đơn
        public async Task DeleteAsync(int soHD, int maSP, int maKho)
        {
            var cthd = await _context.CTHD
                .FirstOrDefaultAsync(ct => ct.SoHD == soHD && ct.MaSP == maSP && ct.MaKho == maKho);
            if (cthd != null)
            {
                _context.CTHD.Remove(cthd);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Chi tiết hóa đơn với SoHD={soHD}, MaSP={maSP}, MaKho={maKho} không tồn tại.");
            }
        }

        public async Task<IEnumerable<CTHD>> GetByMaKHAndMaSPAsync(int maKH, int maSP)
        {
            return await _context.CTHD
                .Include(ct => ct.HoaDon)
                .Include(ct => ct.TonKho)
                .ThenInclude(tk => tk.SanPham)
                .Include(ct => ct.TonKho)
                .ThenInclude(tk => tk.Kho)
                .Where(ct => ct.HoaDon.MaKH == maKH && ct.MaSP == maSP)

                .ToListAsync();
        }

    }
}