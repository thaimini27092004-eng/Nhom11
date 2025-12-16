
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Models.QuanLyThongKe;
using WebsiteBanHang.Models.QuanLyTrangThai;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Repositories.LopThucThi.QLTrangThai;

namespace WebsiteBanHang.Repositories.EF
{
    // Lớp EFHoaDonRepository triển khai giao diện IHoaDonCategory
    public class EFHoaDonRepository : IHoaDonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;

        // Constructor nhận một đối tượng ApplicationDbContext để tương tác với cơ sở dữ liệu
        public EFHoaDonRepository(ApplicationDbContext context,
            ILichSuTTHDRepository lichSuTTHDRepository)
        {
            _context = context;
            _lichSuTTHDRepository = lichSuTTHDRepository;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        // Lấy tất cả hóa đơn
        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            var hoaDons = await _context.HoaDon
                .Include(hd => hd.PTTT)
                .Include(hd => hd.KhachHang)
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
        //---------------------------------------------------------------------------------------------------------------------------------------
        // Lấy hóa đơn theo ID
        public async Task<HoaDon> GetByIdAsync(int maHD)
        {
            var hoaDon = await _context.HoaDon
                .Include(hd => hd.PTTT)
                .Include(hd => hd.KhachHang)
                .Include(hd => hd.CTHDs)
                .ThenInclude(ct => ct.TonKho)
                .ThenInclude(tk => tk.SanPham)
                .Include(hd => hd.CTHDs)
                .ThenInclude(ct => ct.TonKho)
                .ThenInclude(tk => tk.Kho)
                .Include(hd => hd.LichSuTTHD)
                .ThenInclude(ls => ls.TrangThai)
                .FirstOrDefaultAsync(hd => hd.SoHD == maHD);

            if (hoaDon == null)
            {
                throw new KeyNotFoundException($"Hóa đơn với ID {maHD} không tồn tại.");
            }

            //var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(hoaDon.SoHD);
            // hoaDon.LichSuTTHD = new List<LichSuTTHD> { latestStatus };
            if (hoaDon.LichSuTTHD != null)
            {
                hoaDon.LichSuTTHD = hoaDon.LichSuTTHD.OrderBy(ls => ls.ThoiGianThayDoi).ToList();
            }

            return hoaDon;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        // Thêm hóa đơn mới
        public async Task AddAsync(HoaDon hoaDon)
        {
            // Kiểm tra nếu đối tượng hoaDon là null
            if (hoaDon == null)
            {
                throw new ArgumentNullException(nameof(hoaDon)); // Ném ra ngoại lệ nếu null
            }

            // Thêm hóa đơn vào DbSet và lưu thay đổi
            _context.HoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        // Cập nhật hóa đơn
        public async Task UpdateAsync(HoaDon hoaDon)
        {
            // Kiểm tra nếu đối tượng hoaDon là null
            if (hoaDon == null)
            {
                throw new ArgumentNullException(nameof(hoaDon)); // Ném ra ngoại lệ nếu null
            }

            // Cập nhật hóa đơn trong DbSet và lưu thay đổi
            _context.HoaDon.Update(hoaDon);
            await _context.SaveChangesAsync();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        // Xóa hóa đơn
        public async Task DeleteAsync(int maHD)
        {
            // Tìm hóa đơn trong cơ sở dữ liệu theo ID
            var hoaDon = await _context.HoaDon.FindAsync(maHD);
            // Nếu tìm thấy hóa đơn, xóa nó
            if (hoaDon != null)
            {
                _context.HoaDon.Remove(hoaDon);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Nếu không tìm thấy, ném ra ngoại lệ
                throw new KeyNotFoundException($"Hóa đơn với ID {maHD} không tồn tại.");
            }
        }


        //---------------------------------------------------------------------------------------------------------------------------------------



    }
}