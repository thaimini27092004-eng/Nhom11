using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.QuanLyTrangThai;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;

namespace WebsiteBanHang.Repositories.LopThucThi.QuanLyHoaDon
{
    public class EFQuanLyHoaDonRepository : IQuanLyHoaDonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;

        public EFQuanLyHoaDonRepository(ApplicationDbContext context, ILichSuTTHDRepository lichSuTTHDRepository)
        {
            _context = context;
            _lichSuTTHDRepository = lichSuTTHDRepository;
        }

        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            var hoaDons = await _context.HoaDon
                .Include(hd => hd.PTTT)
                .Include(hd => hd.KhachHang)
                .Include(hd => hd.CTHDs)
                .ThenInclude(ct => ct.TonKho)
                .ThenInclude(tk => tk.SanPham)
                .Include(hd => hd.LichSuTTHD)
                .ThenInclude(ls => ls.TrangThai)
                .ToListAsync();

            foreach (var hd in hoaDons)
            {
                var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hd.SoHD);
                hd.LichSuTTHD = new List<LichSuTTHD> { latestStatus };
            }

            return hoaDons;
        }

        public async Task<HoaDon> GetByIdAsync(int soHD)
        {
            var hoaDon = await _context.HoaDon
                .Include(hd => hd.PTTT)
                .Include(hd => hd.KhachHang)
                .Include(hd => hd.CTHDs)
                .ThenInclude(ct => ct.TonKho)
                .ThenInclude(tk => tk.SanPham)
                .Include(hd => hd.LichSuTTHD)
                .ThenInclude(ls => ls.TrangThai)
                .FirstOrDefaultAsync(hd => hd.SoHD == soHD);

            if (hoaDon == null)
            {
                throw new KeyNotFoundException($"Hóa đơn với ID {soHD} không tồn tại.");
            }

            //var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDon.SoHD);
            //hoaDon.LichSuTTHD = new List<LichSuTTHD> { latestStatus };
            // Sắp xếp LichSuTTHD theo thời gian tăng dần để hiển thị timeline đúng thứ tự
            if (hoaDon.LichSuTTHD != null)
            {
                hoaDon.LichSuTTHD = hoaDon.LichSuTTHD.OrderBy(ls => ls.ThoiGianThayDoi).ToList();
            }
            return hoaDon;
        }

        public async Task UpdateAsync(HoaDon hoaDon)
        {
            if (hoaDon == null)
            {
                throw new ArgumentNullException(nameof(hoaDon));
            }
            _context.HoaDon.Update(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int soHD)
        {
            var hoaDon = await _context.HoaDon.FindAsync(soHD);
            if (hoaDon != null)
            {
                _context.HoaDon.Remove(hoaDon);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Hóa đơn với ID {soHD} không tồn tại.");
            }
        }

        public async Task<IEnumerable<int>> GetMaSPByHoaDonAsync(List<int> soHD)
        {
            return await _context.CTHD
                .Where(ct => soHD.Contains(ct.SoHD))
                .Select(ct => ct.MaSP)
                .Distinct()
                .ToListAsync();
        }
    }
}
