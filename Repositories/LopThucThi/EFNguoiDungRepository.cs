using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Repositories.EF
{
    public class EFNguoiDungRepository : INguoiDungRepository
    {
        private readonly ApplicationDbContext _context;

        public EFNguoiDungRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetHoTenByEmailAsync(string email)
        {
            var user = await _context.Users
                .Where(u => u.Email == email) // Tìm theo email
                .Select(u => u.HoTen)
                .FirstOrDefaultAsync();
            return user ?? string.Empty;
        }

        public async Task<string> GetHoTenByUserNameAsync(string userName)
        {
            var user = await _context.Users
                .Where(u => u.UserName == userName) // Tìm theo user name
                .Select(u => u.HoTen)
                .FirstOrDefaultAsync();
            return user ?? string.Empty;
        }

        public async Task<string> GetHoTenBySDTAsync(string sDT)
        {
            var user = await _context.Users
                .Where(u => u.SDT == sDT) // Tìm theo user name
                .Select(u => u.HoTen)
                .FirstOrDefaultAsync();
            return user ?? string.Empty;
        }

        public async Task<ThongTinNguoiDung> FindByEmailOrSDTOrUserNameAsync(string loginInput)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginInput || u.SDT == loginInput || u.UserName == loginInput);
        }
    }
}