using WebsiteBanHang.Models.NguoiDung;
using Microsoft.AspNetCore.Identity;

namespace WebsiteBanHang.Repositories.I
{
    public interface INguoiDungRepository
    {
        Task<string> GetHoTenByEmailAsync(string email);
        Task<string> GetHoTenByUserNameAsync(string userName);
        Task<string> GetHoTenBySDTAsync(string sDT);

        Task<ThongTinNguoiDung> FindByEmailOrSDTOrUserNameAsync(string loginInput);
    }
}