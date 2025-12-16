using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I.QLNhanVien;

namespace WebsiteBanHang.Repositories.LopThucThi.QLNhanVien
{
    public class EFVaiTroNhanVienRepository : IVaiTroNhanVienRepository
    {
        private readonly ApplicationDbContext _context;

        public EFVaiTroNhanVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VaiTroNhanVien vaiTroNhanVien)
        {
            if (vaiTroNhanVien == null)
            {
                throw new ArgumentNullException(nameof(vaiTroNhanVien));
            }

            _context.VaiTroNhanVien.Add(vaiTroNhanVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByMaNVAsync(int maNV)
        {
            var vaiTros = await _context.VaiTroNhanVien
                .Where(vt => vt.MaNV == maNV)
                .ToListAsync();
            if (vaiTros.Count > 0)
            {
                _context.VaiTroNhanVien.RemoveRange(vaiTros);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VaiTroNhanVien>> GetByMaNVAsync(int maNV)
        {
            return await _context.VaiTroNhanVien
                .Where(vt => vt.MaNV == maNV)
                .ToListAsync();
        }
    }
}
