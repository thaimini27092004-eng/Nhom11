using WebsiteBanHang.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WebsiteBanHang.Repositories.I
{
    public interface ICTHDRepository
    {
        Task<IEnumerable<CTHD>> GetAllAsync();
        Task<CTHD> GetByIdAsync(int soHD, int maSP, int maKho);
        Task AddAsync(CTHD cthd);
        Task UpdateAsync(CTHD cthd);
        Task DeleteAsync(int soHD, int maSP, int maKho);
        //lấy danh sách CTHD theo MaKH và MaSP.
        Task<IEnumerable<CTHD>> GetByMaKHAndMaSPAsync(int maKH, int maSP);
    }
}