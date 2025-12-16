using WebsiteBanHang.Models.QuanLyThongKe;

namespace WebsiteBanHang.Repositories.I.QuanLyThongKe
{
    public interface IThongKeRepository
    {
        Task<List<ThongKeViewModel>> GetThongKeAsync(DateTime? startDate, DateTime? endDate);

    }
}
