using WebsiteBanHang.Models.QLTonKho;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.LopThucThi.QLKho
{
    public class EFTonKhoRepository : ITonKhoRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTonKhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TonKho>> GetAllAsync()
        {
            return await _context.TonKho
                .Include(tk => tk.Kho)
                .Include(tk => tk.SanPham)
                .ToListAsync();
        }

        public async Task<TonKho> GetByIdAsync(int maKho, int maSP)
        {
            var tonKho = await _context.TonKho
                .Include(tk => tk.Kho)
                .Include(tk => tk.SanPham)
                .FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaSP == maSP);

            if (tonKho == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tồn kho với MaKho={maKho}, MaSP={maSP}.");
            }

            return tonKho;
        }

        public async Task<IEnumerable<TonKho>> GetByMaSPAsync(int maSP)
        {
            return await _context.TonKho
                .Include(tk => tk.Kho)
                .Include(tk => tk.SanPham)
                .Where(tk => tk.MaSP == maSP)
                .ToListAsync();
        }



        public async Task AddAsync(TonKho tonKho)
        {
            if (tonKho == null)
            {
                throw new ArgumentNullException(nameof(tonKho));
            }

            _context.TonKho.Add(tonKho);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TonKho tonKho)
        {
            if (tonKho == null)
            {
                throw new ArgumentNullException(nameof(tonKho));
            }

            var existingTonKho = await _context.TonKho
                .FirstOrDefaultAsync(tk => tk.MaKho == tonKho.MaKho && tk.MaSP == tonKho.MaSP);
            if (existingTonKho == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tồn kho với MaKho={tonKho.MaKho}, MaSP={tonKho.MaSP}.");
            }

            _context.Entry(existingTonKho).CurrentValues.SetValues(tonKho);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maKho, int maSP)
        {
            var tonKho = await _context.TonKho
                .FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaSP == maSP);
            if (tonKho != null)
            {
                _context.TonKho.Remove(tonKho);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Không tìm thấy tồn kho với MaKho={maKho}, MaSP={maSP}.");
            }
        }

        public async Task<TonKho> FindKhoWithEnoughStockAsync(int maSP, int quantity)
        {
            return await _context.TonKho
                .Include(tk => tk.Kho)
                .Include(tk => tk.SanPham)
                .Where(tk => tk.MaSP == maSP && tk.SLTon >= quantity)
                .FirstOrDefaultAsync();
        }
    }
}