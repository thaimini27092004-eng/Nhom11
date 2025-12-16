using WebsiteBanHang.Models.NguoiDung;

namespace WebsiteBanHang.Repositories.I.QLGmailThongBao
{
    public interface IGmailThongBaoRepository
    {
        Task ThemThongBaoAsync(GmailThongBao thongBao);
        Task<GmailThongBao> LayThongBaoTheoMaAsync(int maTB);
        Task CapNhatThongBaoAsync(GmailThongBao thongBao);
        Task XoaThongBaoAsync(int maTB);
    }
}
