using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories.I.QLNhanVien
{
    public interface IVaiTroNhanVienRepository
    {
        Task AddAsync(VaiTroNhanVien vaiTroNhanVien);
        Task DeleteByMaNVAsync(int maNV);
        Task<IEnumerable<VaiTroNhanVien>> GetByMaNVAsync(int maNV);
    }
}
