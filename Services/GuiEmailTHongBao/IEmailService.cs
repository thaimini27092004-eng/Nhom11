namespace WebsiteBanHang.Services.GuiEmailTHongBao
{
    public interface IEmailService
    {
        Task GuiEmailAsync(string emailNguoiNhan, string tieuDe, string noiDungHtml);
        Task GuiEmailDatHangThanhCongAsync(int soHD, string emailKhachHang);
        Task GuiEmailCapNhatTrangThaiAsync(int soHD, string emailKhachHang, string tenTrangThaiMoi);
    }
}
