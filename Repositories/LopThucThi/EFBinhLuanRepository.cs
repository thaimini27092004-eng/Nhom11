using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFBinhLuanRepository : IBinhLuanRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBinhLuanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Lấy tất cả phản hồi (đánh giá + thông tin khách hàng)
        public async Task<IEnumerable<BinhLuan>> GetAllAsync()
        {
            return await _context.BinhLuan
                .Include(ph => ph.KhachHang) // Include thông tin KhachHang
                .ToListAsync();
        }

        //Lấy phản hồi theo MaPH
        public async Task<BinhLuan> GetByIdAsync(int maPH)
        {
            var phanHoi = await _context.BinhLuan
                .Include(ph => ph.KhachHang)
                .FirstOrDefaultAsync(ph => ph.MaBL == maPH);

            if (phanHoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPH={maPH}.");
            }

            return phanHoi;
        }

        //truy vấn tất cả phản hồi của một khách hàn qua MaKH
        public async Task<IEnumerable<BinhLuan>> GetByMaKHAsync(int maKH)
        {
            return await _context.BinhLuan
                .Include(ph => ph.KhachHang)
                .Where(ph => ph.MaKH == maKH)
                .ToListAsync();
        }

        //truy vấn tất cả phản hồi của một đánh giá qua MaDG
        public async Task<IEnumerable<BinhLuan>> GetByMaDGAsync(int maDG)
        {
            return await _context.BinhLuan
                .Include(ph => ph.KhachHang)
                .ToListAsync();
        }

        //truy vấn tất cả phản hồi của một sản phẩm qua MaSP
        public async Task<IEnumerable<BinhLuan>> GetByMaSPAsync(int maSP)
        {
            try
            {
                return await _context.BinhLuan
                    .Where(ph => ph.MaSP == maSP)
                    .Select(ph => new BinhLuan
                    {
                        MaBL = ph.MaBL,
                        MaKH = ph.MaKH,
                        MaSP = ph.MaSP,
                        NoiDungBL = ph.NoiDungBL,
                        NgayBL = ph.NgayBL,
                        KhachHang = new KhachHang { TenKH = ph.KhachHang.TenKH, AvatarUrl = ph.KhachHang.AvatarUrl }
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phản hồi cho MaSP={maSP}", ex);
            }
        }

        //Thêm 1 phản hồi
        public async Task AddAsync(BinhLuan phanHoi)
        {
            if (phanHoi == null)
            {
                throw new ArgumentNullException(nameof(phanHoi));
            }

            _context.BinhLuan.Add(phanHoi);
            await _context.SaveChangesAsync();
        }


        //sửa 1 phảm hồi
        public async Task UpdateAsync(BinhLuan phanHoi)
        {
            if (phanHoi == null)
            {
                throw new ArgumentNullException(nameof(phanHoi));
            }

            var existingPhanHoi = await _context.BinhLuan
                .FirstOrDefaultAsync(ph => ph.MaBL == phanHoi.MaBL);
            if (existingPhanHoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPH={phanHoi.MaBL}.");
            }

            _context.Entry(existingPhanHoi).CurrentValues.SetValues(phanHoi);
            await _context.SaveChangesAsync();
        }

        //Xoa 1 phản hồi
        public async Task DeleteAsync(int maPH)
        {
            var phanHoi = await _context.BinhLuan
                .FirstOrDefaultAsync(ph => ph.MaBL == maPH);
            if (phanHoi != null)
            {
                _context.BinhLuan.Remove(phanHoi);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy phản hồi với MaPH={maPH}.");
            }
        }

        public async Task<IEnumerable<BinhLuan>> GetByMaSPPagedAsync(int maSP, int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            return await _context.BinhLuan
                .Include(ph => ph.KhachHang)
                .Where(ph => ph.MaSP == maSP)
                .GroupJoin(_context.LikeBinhLuan,
                    ph => ph.MaBL,
                    lph => lph.MaBL,
                    (ph, lphs) => new { BinhLuan = ph, SoLuotThich = lphs.Count() })
                .OrderByDescending(x => x.SoLuotThich)
                .ThenByDescending(x => x.BinhLuan.NgayBL)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => x.BinhLuan)
                .ToListAsync();
        }



    }
}