using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFKhachHangRepository : IKhachHangRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKhachHangRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả khách hàng
        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            return await _context.KhachHang
                .Include(kh => kh.User)
                .Include(kh => kh.HoaDons)
                .Include(kh => kh.GioHang)
                .ToListAsync();
        }

        // Lấy khách hàng theo MaKH
        public async Task<KhachHang> GetByIdAsync(int maKH)
        {
            var khachHang = await _context.KhachHang
                .Include(kh => kh.User)
                .Include(kh => kh.GioHang)
                .FirstOrDefaultAsync(kh => kh.MaKH == maKH);

            if (khachHang == null)
            {
                throw new KeyNotFoundException($"Khách hàng với ID {maKH} không tồn tại.");
            }

            return khachHang;
        }

        // Lấy khách hàng theo UserId
        public async Task<KhachHang> GetByUserIdAsync(string userId)
        {
            var khachHang = await _context.KhachHang
                .Include(kh => kh.User)
                .Include(kh => kh.HoaDons)
                .Include(kh => kh.GioHang)
                .FirstOrDefaultAsync(kh => kh.UserId == userId);

            if (khachHang == null)
            {
                throw new KeyNotFoundException($"Khách hàng với UserId {userId} không tồn tại.");
            }

            return khachHang;
        }

        // Lấy danh sách SoHD của hóa đơn theo UserId
       public async Task<List<int>> GetHoaDonIdsByUserIdAsync(string userId)
        {
            var soHDList = await _context.HoaDon
                .Where(hd => _context.KhachHang
                    .Where(kh => kh.UserId == userId)
                    .Select(kh => kh.MaKH)
                    .Contains(hd.MaKH))
                .Select(hd => hd.SoHD)
                .Distinct()
                .ToListAsync();

            return soHDList;
        }

        public async Task<List<int>> GetHoaDonIdsByMaKHsAsync(List<int> maKHs)
        {
            if (maKHs == null || !maKHs.Any())
            {
                return new List<int>();
            }

            var soHDList = await _context.HoaDon
                .Where(hd => maKHs.Contains(hd.MaKH))
                .Select(hd => hd.SoHD)
                .Distinct()
                .ToListAsync();

            return soHDList;
        }

        // Thêm khách hàng mới
        public async Task AddAsync(KhachHang khachHang)
        {
            if (khachHang == null)
            {
                throw new ArgumentNullException(nameof(khachHang));
            }

            _context.KhachHang.Add(khachHang);
            await _context.SaveChangesAsync();
        }

        // Cập nhật khách hàng
        public async Task UpdateAsync(KhachHang khachHang)
        {
            if (khachHang == null)
            {
                throw new ArgumentNullException(nameof(khachHang));
            }

            _context.KhachHang.Update(khachHang);
            await _context.SaveChangesAsync();
        }

        // Xóa khách hàng
        public async Task DeleteAsync(int maKH)
        {
            var khachHang = await _context.KhachHang.FindAsync(maKH);
            if (khachHang != null)
            {
                _context.KhachHang.Remove(khachHang);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Khách hàng với ID {maKH} không tồn tại.");
            }
        }
    }
}