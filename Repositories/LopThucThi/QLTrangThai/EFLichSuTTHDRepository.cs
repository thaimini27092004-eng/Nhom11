using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QuanLyTrangThai;
using WebsiteBanHang.Repositories.I.QLTrangThai;

namespace WebsiteBanHang.Repositories.LopThucThi.QLTrangThai
{
    public class EFLichSuTTHDRepository : ILichSuTTHDRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLichSuTTHDRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LichSuTTHD>> GetAllAsync()
        {
            return await _context.LichSuTTHD
                .Include(ls => ls.TrangThai)
                .Include(ls => ls.HoaDon)
                .ToListAsync();
        }

        public async Task<LichSuTTHD> GetByIdAsync(int maLS)
        {
            var lichSu = await _context.LichSuTTHD
                .Include(ls => ls.TrangThai)
                .Include(ls => ls.HoaDon)
                .FirstOrDefaultAsync(ls => ls.MaLS == maLS);
            if (lichSu == null)
            {
                throw new KeyNotFoundException($"Lịch sử với ID {maLS} không tồn tại.");
            }
            return lichSu;
        }

        public async Task<IEnumerable<LichSuTTHD>> GetBySoHDAsync(int soHD)
        {
            return await _context.LichSuTTHD
                .Include(ls => ls.TrangThai)
                .Include(ls => ls.HoaDon)
                .Where(ls => ls.SoHD == soHD)
                .OrderBy(ls => ls.ThoiGianThayDoi)
                .ToListAsync();
        }

        public async Task<LichSuTTHD> GetLatestBySoHDAsync(int soHD)
        {
            var lichSu = await _context.LichSuTTHD
                .Include(ls => ls.TrangThai)
                .Include(ls => ls.HoaDon)
                .Where(ls => ls.SoHD == soHD)
                .OrderByDescending(ls => ls.ThoiGianThayDoi)
                .FirstOrDefaultAsync();
            if (lichSu == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lịch sử trạng thái cho hóa đơn {soHD}.");
            }
            return lichSu;
        }

        public async Task AddAsync(LichSuTTHD lichSu)
        {
            if (lichSu == null)
            {
                throw new ArgumentNullException(nameof(lichSu));
            }
            _context.LichSuTTHD.Add(lichSu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LichSuTTHD lichSu)
        {
            if (lichSu == null)
            {
                throw new ArgumentNullException(nameof(lichSu));
            }
            _context.LichSuTTHD.Update(lichSu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maLS)
        {
            var lichSu = await _context.LichSuTTHD.FindAsync(maLS);
            if (lichSu != null)
            {
                _context.LichSuTTHD.Remove(lichSu);
                await _context.SaveChangesAsync();
            }
        }
    }
}
