using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories.I.QLGmailThongBao;

namespace WebsiteBanHang.Repositories.LopThucThi.QLGmailThongBao
{
    public class EFGmailThongBaoRepository : IGmailThongBaoRepository
    {
        private readonly ApplicationDbContext _context;

        public EFGmailThongBaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ThemThongBaoAsync(GmailThongBao thongBao)
        {
            if (thongBao == null)
            {
                throw new ArgumentNullException(nameof(thongBao));
            }

            _context.GmailThongBao.Add(thongBao);
            await _context.SaveChangesAsync();
        }

        public async Task<GmailThongBao> LayThongBaoTheoMaAsync(int maTB)
        {
            var thongBao = await _context.GmailThongBao
                .Include(tb => tb.LichSuTTHD)
                .FirstOrDefaultAsync(tb => tb.MaTB == maTB);

            if (thongBao == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy thông báo với MaTB {maTB}.");
            }

            return thongBao;
        }

        public async Task CapNhatThongBaoAsync(GmailThongBao thongBao)
        {
            if (thongBao == null)
            {
                throw new ArgumentNullException(nameof(thongBao));
            }

            _context.GmailThongBao.Update(thongBao);
            await _context.SaveChangesAsync();
        }

        public async Task XoaThongBaoAsync(int maTB)
        {
            var thongBao = await _context.GmailThongBao.FindAsync(maTB);
            if (thongBao != null)
            {
                _context.GmailThongBao.Remove(thongBao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
