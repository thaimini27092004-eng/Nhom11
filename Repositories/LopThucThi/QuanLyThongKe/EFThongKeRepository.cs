using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.QuanLyThongKe;
using WebsiteBanHang.Repositories.I.QuanLyThongKe;

namespace WebsiteBanHang.Repositories.LopThucThi.QuanLyThongKe
{
    public class EFThongKeRepository : IThongKeRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor nhận một đối tượng ApplicationDbContext để tương tác với cơ sở dữ liệu
        public EFThongKeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Phương thức thống kê
        public async Task<List<ThongKeViewModel>> GetThongKeAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = from hd in _context.HoaDon
                        join cthd in _context.CTHD on hd.SoHD equals cthd.SoHD
                        select new
                        {
                            NgayDat = hd.NgayDat,
                            SoLuong = cthd.SL,
                            DonGia = cthd.DonGia,
                            SoHD = hd.SoHD
                        };

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.NgayDat >= startDate.Value && x.NgayDat <= endDate.Value);
            }

            var groupedData = await query
                .GroupBy(x => x.NgayDat.Date)
                .Select(g => new
                {
                    NgayDat = g.Key,
                    SoLuongBan = g.Sum(x => x.SoLuong),
                    SoLuongDonHang = g.Select(x => x.SoHD).Distinct().Count(),
                    DoanhThu = g.Sum(x => (long)x.SoLuong * x.DonGia)
                })
                .OrderBy(x => x.NgayDat)
                .ToListAsync();

            // Debug: Kiểm tra dữ liệu sau khi nhóm
            foreach (var item in groupedData)
            {
                Console.WriteLine($"NgayDat: {item.NgayDat}, SoLuongBan: {item.SoLuongBan}, SoLuongDonHang: {item.SoLuongDonHang}, DoanhThu: {item.DoanhThu}");
            }

            var result = groupedData.Select(x => new ThongKeViewModel
            {
                NgayDatHang = x.NgayDat.ToString("dd-MM-yyyy"),
                SoLuongBan = x.SoLuongBan,
                SoLuongDonHang = x.SoLuongDonHang,
                DoanhThu = Math.Round((double)x.DoanhThu / 1000000, 1)
            }).ToList();

            return result;
        }
    }
}
