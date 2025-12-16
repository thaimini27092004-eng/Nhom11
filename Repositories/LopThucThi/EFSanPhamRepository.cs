using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFSanPhamRepository : ISanPhamRepository
    {
        private readonly ApplicationDbContext _context;
        public EFSanPhamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //láy toàn bộ danh sách sản phẩm
        public async Task<IEnumerable<SanPham>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.SanPham
            .Include(p => p.DanhMuc) // Include thêm thông tin về category
            .Include(p => p.DSAnh) // Include thêm danh sách ảnh
            .ToListAsync();//
        }

        //lấy 1 sản phẩm theo id
        public async Task<SanPham> GetByIdAsync(int id)
        {
            // lấy thông tin kèm theo category
                 return await _context.SanPham
                .Include(p => p.DanhMuc)
                .Include(p => p.DSAnh) // Include danh sách ảnh
                .FirstOrDefaultAsync(p => p.MaSP == id); //Tìm sản phẩm có Id khớp với id, lấy bản ghi đầu tiên
        }


        //bất đồng bộ lấy danh sách sản phẩm theo danh sách id
        public async Task<List<SanPham>> GetByIdsAsync(List<int> ids)
        {
            return await _context.SanPham
                .Include(p => p.DanhMuc)
                .Include(p => p.DSAnh)
                .Where(p => ids.Contains(p.MaSP))
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPham>> GetByMaKhoAsync(int maKho)
        {
            return await _context.TonKho
                .Where(tk => tk.MaKho == maKho)
                .Include(tk => tk.SanPham)
                .ThenInclude(sp => sp.DanhMuc)
                .Include(tk => tk.SanPham)
                .ThenInclude(sp => sp.DSAnh)
                .Select(tk => tk.SanPham)
                .Distinct()
                .ToListAsync();
        }

        public async Task AddAsync(SanPham sanpham)
        {
            _context.SanPham.Add(sanpham);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(SanPham sanpham)
        {
            _context.SanPham.Update(sanpham);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var sanpham = await _context.SanPham
                .Include(p => p.DSAnh) // Bao gồm danh sách ảnh bổ sung
                .FirstOrDefaultAsync(p => p.MaSP == id);

            if (sanpham == null)
            {
                return; // Nếu không tìm thấy sản phẩm, thoát
            }

            // Xóa ảnh đại diện (UrlAnh)
            if (!string.IsNullOrEmpty(sanpham.UrlAnh))
            {
                var imagePath = Path.Combine("wwwroot", sanpham.UrlAnh.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Xóa các ảnh bổ sung (DSAnh)
            if (sanpham.DSAnh != null && sanpham.DSAnh.Any())
            {
                foreach (var dsAnh in sanpham.DSAnh)
                {
                    var imagePath = Path.Combine("wwwroot", dsAnh.UrlAnh.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            // Xóa sản phẩm khỏi cơ sở dữ liệu
            _context.SanPham.Remove(sanpham);
            await _context.SaveChangesAsync();
        }
    }
}
