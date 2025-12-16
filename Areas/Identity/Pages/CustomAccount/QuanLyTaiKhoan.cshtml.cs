using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Repositories.I.QLTrangThai;

namespace WebsiteBanHang.Areas.Identity.Pages.CustomAccount
{
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập truy cập
    public class QuanLyTaiKhoanModel : PageModel
    {
        // Các công cụ cần thiết để làm việc
        private readonly UserManager<ThongTinNguoiDung> _nguoiDungManager; // Quản lý thông tin người dùng
        private readonly SignInManager<ThongTinNguoiDung> _dangNhapManager; // Quản lý đăng nhập
        private readonly ApplicationDbContext _duLieu; // Kết nối với cơ sở dữ liệu
        private readonly ILichSuTTHDRepository _lichSuTTHDRepository;

        // Khởi tạo các công cụ
        public QuanLyTaiKhoanModel(UserManager<ThongTinNguoiDung> nguoiDungManager,
            SignInManager<ThongTinNguoiDung> dangNhapManager,
            ApplicationDbContext duLieu,
            ILichSuTTHDRepository lichSuTTHDRepository)
        {
            _nguoiDungManager = nguoiDungManager;
            _dangNhapManager = dangNhapManager;
            _duLieu = duLieu;
            _lichSuTTHDRepository = lichSuTTHDRepository;
        }

        // Dữ liệu người dùng nhập vào
        [BindProperty]
        public InputModel ThongTinNhap { get; set; }

        // Định nghĩa thông tin người dùng cần nhập
        public class InputModel
        {
            public string HoTen { get; set; }
            public DateTime? NgaySinh { get; set; } // Thêm trường ngày sinh, không bắt buộc
            public string DiaChi { get; set; }
            public string SDT { get; set; }
        }

        // Hiển thị trang khi người dùng truy cập
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Lấy thông tin người dùng hiện tại từ bảng AspNetUsers
            var nguoiDungHienTai = await _nguoiDungManager.GetUserAsync(User);
            // Vì đã có [Authorize], người dùng chắc chắn tồn tại, nên không cần kiểm tra null

            // Điền thông tin hiện tại vào form để hiển thị
            ThongTinNhap = new InputModel
            {
                HoTen = nguoiDungHienTai.HoTen ?? string.Empty, // Nếu không có tên, để trống
                NgaySinh = nguoiDungHienTai.NgaySinh, // Hiển thị ngày sinh hiện tại (nếu có)
                DiaChi = nguoiDungHienTai.DiaChi ?? string.Empty, // Nếu không có địa chỉ, để trống
                SDT = nguoiDungHienTai.SDT ?? string.Empty // Nếu không có số điện thoại, để trống
            };
            ViewData["ReturnUrl"] = returnUrl;
            return Page(); // Hiển thị trang
        }

        // Xử lý khi người dùng gửi thông tin để cập nhật
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Bước 1: Tự kiểm tra dữ liệu nhập vào
            // Kiểm tra HoTen
            if (string.IsNullOrEmpty(ThongTinNhap.HoTen))
            {
                TempData["StatusMessage"] = "Thất bại: Tên không được để trống.";
                return Page(); // Giữ trên trang để người dùng sửa
            }

            // Kiểm tra DiaChi và SDT
            bool coDiaChi = !string.IsNullOrEmpty(ThongTinNhap.DiaChi); // Kiểm tra xem có nhập địa chỉ không
            bool coSDT = !string.IsNullOrEmpty(ThongTinNhap.SDT); // Kiểm tra xem có nhập số điện thoại không

            // Nếu một trong hai trường có dữ liệu, thì cả hai phải có dữ liệu
            if (coDiaChi || coSDT)
            {
                if (!coSDT) // Nếu SDT trống nhưng DiaChi có dữ liệu
                {
                    TempData["StatusMessage"] = "Thất bại: Số điện thoại không được để trống.";
                    return Page(); // Giữ trên trang để sửa
                }
                if (!coDiaChi) // Nếu DiaChi trống nhưng SDT có dữ liệu
                {
                    TempData["StatusMessage"] = "Thất bại: Địa chỉ không được để trống.";
                    return Page(); // Giữ trên trang để sửa
                }
            }

            // Bước 2: Lấy thông tin người dùng hiện tại từ bảng AspNetUsers
            var nguoiDungHienTai = await _nguoiDungManager.GetUserAsync(User);
            // Vì đã có [Authorize], người dùng chắc chắn tồn tại, nên không cần kiểm tra null

            // Bước 3: Kiểm tra xem có thay đổi gì không (tên, ngày sinh, địa chỉ, hoặc số điện thoại)
            bool coThayDoi = false;

            // Kiểm tra thay đổi tên
            if (!string.Equals(nguoiDungHienTai.HoTen, ThongTinNhap.HoTen))
            {
                nguoiDungHienTai.HoTen = ThongTinNhap.HoTen; // Cập nhật tên
                coThayDoi = true; // Đánh dấu có thay đổi
            }

            // Kiểm tra thay đổi ngày sinh
            if (nguoiDungHienTai.NgaySinh != ThongTinNhap.NgaySinh)
            {
                nguoiDungHienTai.NgaySinh = ThongTinNhap.NgaySinh; // Cập nhật ngày sinh (có thể là null)
                coThayDoi = true; // Đánh dấu có thay đổi
            }

            // Kiểm tra thay đổi địa chỉ
            if (!string.Equals(nguoiDungHienTai.DiaChi, ThongTinNhap.DiaChi))
            {
                nguoiDungHienTai.DiaChi = string.IsNullOrEmpty(ThongTinNhap.DiaChi) ? null : ThongTinNhap.DiaChi; // Nếu trống thì gán null
                coThayDoi = true; // Đánh dấu có thay đổi
            }

            // Kiểm tra thay đổi số điện thoại
            if (!string.Equals(nguoiDungHienTai.SDT, ThongTinNhap.SDT))
            {
                nguoiDungHienTai.SDT = string.IsNullOrEmpty(ThongTinNhap.SDT) ? null : ThongTinNhap.SDT; // Nếu trống thì gán null
                coThayDoi = true; // Đánh dấu có thay đổi
            }

            // Bước 4: Xử lý cập nhật
            // Nếu có thay đổi, cập nhật cả AspNetUsers và KhachHangs
            if (coThayDoi)
            {
                // Cập nhật thông tin vào bảng AspNetUsers
                var ketQuaCapNhat = await _nguoiDungManager.UpdateAsync(nguoiDungHienTai);
                if (!ketQuaCapNhat.Succeeded)
                {
                    // Debug: In ra lỗi từ UserManager nếu có
                    foreach (var error in ketQuaCapNhat.Errors)
                    {
                        Console.WriteLine($"UserManager Error: {error.Description}");
                    }
                    TempData["StatusMessage"] = "Thất bại: Không thể cập nhật thông tin người dùng.";
                    return Page(); // Giữ trên trang
                }
            }

            // Bước 5: Luôn cập nhật KhachHangs, bất kể có thay đổi hay không
            var khachHang = await _duLieu.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == nguoiDungHienTai.Id);

            // Nếu có bản ghi trong KhachHangs, thì cập nhật thông tin
            if (khachHang != null)
            {
                // Đồng bộ thông tin từ ThongTinNguoiDung vào KhachHangs
                khachHang.TenKH = nguoiDungHienTai.HoTen; // Đồng bộ tên
                khachHang.NgaySinhKH = nguoiDungHienTai.NgaySinh; // Đồng bộ ngày sinh
                khachHang.DiaChiKH = nguoiDungHienTai.DiaChi; // Đồng bộ địa chỉ
                khachHang.DTKH = nguoiDungHienTai.SDT; // Đồng bộ số điện thoại
                khachHang.EmailKH = nguoiDungHienTai.Email; // Đồng bộ email
                try
                {
                    await _duLieu.SaveChangesAsync(); // Lưu thay đổi vào bảng KhachHangs
                }
                catch (Exception ex)
                {
                    // Debug: In ra lỗi từ Entity Framework nếu có
                    Console.WriteLine($"Entity Framework Error: {ex.Message}");
                    TempData["StatusMessage"] = "Thất bại: Không thể cập nhật thông tin khách hàng.";
                    return Page();
                }
            }

            //b6: Cập nhật thông tin hóa đơn nếu có returnUrl
            if (coDiaChi && coSDT && !string.IsNullOrEmpty(returnUrl))
            {
                // Chuyển returnUrl thành chuỗi
                string url = returnUrl;

                // Tạo mảng để lưu các chữ số
                int[] soHoaDonMang = new int[10]; // Giả sử số hóa đơn tối đa 10 chữ số
                int viTri = 0;

                // Duyệt từ cuối chuỗi lên đầu
                for (int i = url.Length - 1; i >= 0; i--)
                {
                    char kyTu = url[i];

                    // Nếu gặp dấu '/', dừng lại
                    if (kyTu == '/')
                    {
                        break;
                    }

                    // Nếu là chữ số, lưu vào mảng
                    if (char.IsDigit(kyTu))
                    {
                        soHoaDonMang[viTri] = kyTu - '0'; // Chuyển ký tự thành số (ví dụ: '6' -> 6)
                        viTri++;
                    }
                }

                // Ghép các chữ số lại thành số hóa đơn
                int soHD = 0;
                for (int j = viTri - 1; j >= 0; j--)
                {
                    soHD = soHD * 10 + soHoaDonMang[j];
                }

                // Cập nhật hóa đơn nếu tìm thấy
                if (soHD > 0)
                {
                    // Kiểm tra trạng thái mới nhất từ LichSuTTHD
                    var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(soHD);
                    var hoaDon = await _duLieu.HoaDon.FindAsync(soHD);
                    if (hoaDon != null && (latestStatus.MaTT == 1 && hoaDon.SoLanDoiTT == 0))
                    {
                        hoaDon.DiaChiGiaoHang = ThongTinNhap.DiaChi;
                        hoaDon.DTNhanHang = ThongTinNhap.SDT;
                        hoaDon.SoLanDoiTT += 1;

                        try
                        {
                            await _duLieu.SaveChangesAsync();
                            TempData["StatusMessage"] = "Thành công: Đã cập nhật thông tin giao hàng cho hóa đơn #" + soHD;
                        }
                        catch (Exception ex)
                        {
                            TempData["StatusMessage"] = "Thất bại: Không thể cập nhật thông tin hóa đơn.";
                            return Page();
                        }
                    }
                }
            }

            // Bước 7: Thông báo kết quả và làm mới phiên đăng nhập
            await _dangNhapManager.RefreshSignInAsync(nguoiDungHienTai);
            if (coThayDoi || (!string.IsNullOrEmpty(returnUrl) && coDiaChi && coSDT))
            {
                if (string.IsNullOrEmpty(TempData["StatusMessage"]?.ToString()))
                {
                    TempData["StatusMessage"] = "Thành công";
                }
            }
            else
            {
                TempData["StatusMessage"] = "Không có thay đổi để cập nhật.";
            }

            return Page();
        }
    }
}