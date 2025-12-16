using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models.VaiTro;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebsiteBanHang.Repositories.I;
using WebsiteBanHang.Repositories.I.QLNhanVien;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class QuanLyNguoiDungController : Controller
    {
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ThongTinNguoiDung> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IVaiTroNhanVienRepository _vaiTroNhanVienRepository;


        public QuanLyNguoiDungController(
            UserManager<ThongTinNguoiDung> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ThongTinNguoiDung> signInManager,
            ApplicationDbContext context,
            IKhachHangRepository khachHangRepository,
            IHoaDonRepository hoaDonRepository,
            INhanVienRepository nhanVienRepository,
            IVaiTroNhanVienRepository vaiTroNhanVienRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _khachHangRepository = khachHangRepository;
            _hoaDonRepository = hoaDonRepository;
            _nhanVienRepository = nhanVienRepository;
            _vaiTroNhanVienRepository = vaiTroNhanVienRepository;
        }

        [HttpGet]
        public async Task<IActionResult> PhanQuyen(string tuKhoaTimKiem = "", string chonTrangThai = "active")
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            switch (chonTrangThai)
            {
                case "all":
                    break;
                case "locked":
                    users = users.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTime.UtcNow).ToList();
                    break;
                case "active":
                default:
                    users = users.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTime.UtcNow).ToList();
                    break;
            }

            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var ketQuaTimKiem = new List<ThongTinNguoiDung>();
                var diemSo = new List<int>();

                if (tuKhoaTimKiem.Contains(","))
                {
                    var idArray = tuKhoaTimKiem.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var danhSachIdHopLe = new List<string>();

                    for (int i = 0; i < idArray.Length; i++)
                    {
                        var id = idArray[i].Trim();
                        if (!string.IsNullOrEmpty(id))
                        {
                            danhSachIdHopLe.Add(id);
                        }
                    }

                    ketQuaTimKiem = users.Where(u => danhSachIdHopLe.Contains(u.Id)).ToList();
                    if (ketQuaTimKiem.Any())
                    {
                        foreach (var user in ketQuaTimKiem)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
                            userRolesViewModel.Add(new UserRolesViewModel
                            {
                                UserId = user.Id,
                                Email = user.Email,
                                HoTen = user.HoTen,
                                Roles = roles.ToList(),
                                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList(),
                                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                            });
                        }
                        return View(userRolesViewModel);
                    }
                    else
                    {
                        ViewBag.KetQuaTimKiem = "Không tìm thấy người dùng với ID đã nhập.";
                    }
                }
                else
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        int diem = TinhSoTuKhop(users[i], tuKhoaArray);
                        if (diem > 0)
                        {
                            ketQuaTimKiem.Add(users[i]);
                            diemSo.Add(diem);
                        }
                    }

                    for (int i = 0; i < ketQuaTimKiem.Count - 1; i++)
                    {
                        for (int j = i + 1; j < ketQuaTimKiem.Count; j++)
                        {
                            if (diemSo[i] < diemSo[j])
                            {
                                var tempUser = ketQuaTimKiem[i];
                                ketQuaTimKiem[i] = ketQuaTimKiem[j];
                                ketQuaTimKiem[j] = tempUser;
                                var tempDiem = diemSo[i];
                                diemSo[i] = diemSo[j];
                                diemSo[j] = tempDiem;
                            }
                        }
                    }

                    if (!ketQuaTimKiem.Any())
                    {
                        ViewBag.KetQuaTimKiem = "Không tìm thấy người dùng liên quan.";
                        users = users.Take(3).ToList();
                    }
                    else
                    {
                        users = ketQuaTimKiem;
                    }
                }
            }

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    HoTen = user.HoTen,
                    Roles = roles.ToList(),
                    AllRoles = _roleManager.Roles.Select(r => r.Name).ToList(),
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                });
            }

            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;
            return View(userRolesViewModel);
        }

        private int TinhSoTuKhop(ThongTinNguoiDung user, string[] tuKhoaArray)
        {
            int soTuKhop = 0;
            var email = (" " + (user.Email?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var hoTen = (" " + (user.HoTen?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in tuKhoaArray)
            {
                if (email.Contains(tuKhoa))
                    soTuKhop += 2;
                else if (hoTen.Contains(tuKhoa))
                    soTuKhop += 1;
            }
            return soTuKhop;
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "")
        {
            var users = await _userManager.Users.ToListAsync();

            switch (chonTrangThai)
            {
                case "all":
                    break;
                case "locked":
                    users = users.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTime.UtcNow).ToList();
                    break;
                case "active":
                default:
                    users = users.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTime.UtcNow).ToList();
                    break;
            }

            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            if (tuKhoa.Contains(","))
            {
                var idArray = tuKhoa.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var danhSachIdHopLe = new List<string>();

                for (int i = 0; i < idArray.Length; i++)
                {
                    var id = idArray[i].Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        danhSachIdHopLe.Add(id);
                    }
                }

                int dem = 0;
                for (int i = 0; i < users.Count && dem < 3; i++)
                {
                    for (int j = 0; j < danhSachIdHopLe.Count; j++)
                    {
                        if (users[i].Id == danhSachIdHopLe[j])
                        {
                            goiY.Add(new
                            {
                                userId = users[i].Id,
                                email = users[i].Email,
                                hoTen = users[i].HoTen
                            });
                            dem++;
                            break;
                        }
                    }
                }
                return Json(goiY);
            }

            for (int i = 0; i < users.Count; i++)
            {
                int diem = TinhSoKyTuKhop(users[i], tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new
                    {
                        userId = users[i].Id,
                        email = users[i].Email,
                        hoTen = users[i].HoTen
                    });
                    diemSo.Add(diem);
                }
            }

            for (int i = 0; i < goiY.Count - 1; i++)
            {
                for (int j = i + 1; j < goiY.Count; j++)
                {
                    if (diemSo[i] < diemSo[j])
                    {
                        var tempGoiY = goiY[i];
                        goiY[i] = goiY[j];
                        goiY[j] = tempGoiY;
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            return Json(goiY.Take(3));
        }

        private int TinhSoKyTuKhop(ThongTinNguoiDung user, string tuKhoa)
        {
            int soKyTuKhop = 0;
            var email = user.Email?.ToLower() ?? "";
            var hoTen = user.HoTen?.ToLower() ?? "";
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tu in tuKhoaArray)
            {
                if (email.Contains(tu))
                    soKyTuKhop += tu.Length * 2;
                else if (hoTen.Contains(tu))
                    soKyTuKhop += tu.Length;
            }
            return soKyTuKhop;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return Json(new { currentRoles, allRoles });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanQuyen(string userId, List<string> selectedRoles)
        {
            // Kiểm tra đầu vào
            if (userId == null || userId == "")
            {
                return Json(new { success = false, message = "Không xác định được người dùng." });
            }

            // Lấy thông tin người dùng
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            // Xóa các vai trò cũ của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return Json(new { success = false, message = "Không thể xóa vai trò cũ." });
            }

            // Nếu có vai trò mới được chọn
            if (selectedRoles != null && selectedRoles.Count > 0)
            {
                // Thêm các vai trò mới
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    return Json(new { success = false, message = "Không thể thêm vai trò mới." });
                }

                // Kiểm tra vai trò "Customer"
                bool isCustomer = false;
                for (int i = 0; i < selectedRoles.Count; i++)
                {
                    if (selectedRoles[i] == "Customer")
                    {
                        isCustomer = true;
                        break;
                    }
                }

                // Nếu không phải Customer
                if (!isCustomer)
                {
                    // Kiểm tra xem đã có trong bảng NhanVien chưa
                    var nhanVien = await _nhanVienRepository.GetByUserIdAsync(userId);
                    if (nhanVien == null)
                    {
                        // Thêm mới vào bảng NhanVien
                        nhanVien = new NhanVien
                        {
                            UserId = userId,
                            TenNV = user.HoTen,
                            NgaySinhNV = user.NgaySinh,
                            DiaChiNV = user.DiaChi,
                            DTNV = user.PhoneNumber,
                            EmailNV = user.Email,
                            AvatarUrl = null
                        };
                        await _nhanVienRepository.AddAsync(nhanVien);
                    }

                    // Xóa các vai trò cũ trong VaiTroNhanVien
                    await _vaiTroNhanVienRepository.DeleteByMaNVAsync(nhanVien.MaNV);

                    // Thêm các vai trò mới vào VaiTroNhanVien
                    for (int i = 0; i < selectedRoles.Count; i++)
                    {
                        var vaiTro = new VaiTroNhanVien
                        {
                            MaNV = nhanVien.MaNV,
                            ChucVu = selectedRoles[i]
                        };
                        await _vaiTroNhanVienRepository.AddAsync(vaiTro);
                    }
                }
                else
                {
                    // Nếu chỉ có Customer, xóa khỏi NhanVien và VaiTroNhanVien
                    var nhanVien = await _nhanVienRepository.GetByUserIdAsync(userId);
                    if (nhanVien != null)
                    {
                        await _vaiTroNhanVienRepository.DeleteByMaNVAsync(nhanVien.MaNV);
                        await _nhanVienRepository.DeleteAsync(nhanVien.MaNV);
                    }
                }
            }
            else
            {
                // Nếu không chọn vai trò nào, mặc định là Customer
                var nhanVien = await _nhanVienRepository.GetByUserIdAsync(userId);
                if (nhanVien != null)
                {
                    await _vaiTroNhanVienRepository.DeleteByMaNVAsync(nhanVien.MaNV);
                    await _nhanVienRepository.DeleteAsync(nhanVien.MaNV);
                }
            }

            // Cập nhật lại phiên đăng nhập nếu là người dùng hiện tại
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == userId)
            {
                await _signInManager.RefreshSignInAsync(currentUser);
            }

            return Json(new { success = true, message = $"Đã cập nhật vai trò cho người dùng {user.Email} thành công." });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<string> selectedUserIds, bool isDelete = false)
        {
            if (selectedUserIds == null || selectedUserIds.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một người dùng để xử lý.";
                return RedirectToAction("PhanQuyen");
            }

            var usersToDelete = new List<string>();
            var usersToLock = new List<string>();

            for (int i = 0; i < selectedUserIds.Count; i++)
            {
                var userId = selectedUserIds[i];
                // Kiểm tra xem người dùng có hóa đơn không
                var hoaDonIds = await _khachHangRepository.GetHoaDonIdsByUserIdAsync(userId);
                bool hasHoaDon = hoaDonIds != null && hoaDonIds.Any();

                if (hasHoaDon)
                    usersToLock.Add(userId);
                else
                    usersToDelete.Add(userId);
            }

            if (isDelete)
            {
                for (int i = 0; i < usersToDelete.Count; i++)
                {
                    var user = await _userManager.FindByIdAsync(usersToDelete[i]);
                    if (user != null)
                        await _userManager.DeleteAsync(user);
                }

                for (int i = 0; i < usersToLock.Count; i++)
                {
                    var user = await _userManager.FindByIdAsync(usersToLock[i]);
                    if (user != null)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                        await _userManager.UpdateAsync(user);
                    }
                }

                if (usersToDelete.Count > 0)
                    TempData["Message"] = $"Đã xóa {usersToDelete.Count} người dùng không có hóa đơn.";
                if (usersToLock.Count > 0)
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã khóa {usersToLock.Count} người dùng có hóa đơn.";
            }
            else
            {
                for (int i = 0; i < selectedUserIds.Count; i++)
                {
                    var user = await _userManager.FindByIdAsync(selectedUserIds[i]);
                    if (user != null)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                        await _userManager.UpdateAsync(user);
                    }
                }
                TempData["Message"] = $"Đã khóa {selectedUserIds.Count} người dùng.";
            }

            return RedirectToAction("PhanQuyen");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Người dùng không tồn tại." });

            if (status)
                user.LockoutEnd = null;
            else
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);

            await _userManager.UpdateAsync(user);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == userId)
                await _signInManager.RefreshSignInAsync(currentUser);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddUserViewModel
            {
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ThongTinNguoiDung
                {
                    UserName = model.Email,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                    await _khachHangRepository.AddAsync(new KhachHang { TenKH = user.HoTen, UserId = user.Id });
                    TempData["Message"] = $"Đã thêm người dùng {user.Email} thành công.";
                    return RedirectToAction("PhanQuyen");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            model.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Display(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"Display: userId is null or empty");
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"Display: User not found for userId = {userId}");
                return NotFound();
            }

            var khachHang = await _khachHangRepository.GetByUserIdAsync(userId);
            if (khachHang == null)
            {
                Console.WriteLine($"Display: KhachHang not found for userId = {userId}");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserDetailsViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                HoTen = user.HoTen,
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow,
                Roles = roles.ToList(),
                MaKH = khachHang?.MaKH ?? 0,
                TenKH = khachHang?.TenKH,
                NgaySinhKH = khachHang?.NgaySinhKH,
                DiaChiKH = khachHang?.DiaChiKH,
                DTKH = khachHang?.DTKH,
                EmailKH = khachHang?.EmailKH,
                AvatarUrl = khachHang?.AvatarUrl
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CheckUserInHoaDon(List<string> selectedUserIds)
        {
            var result = new List<object>();

            for (int i = 0; i < selectedUserIds.Count; i++)
            {
                var userId = selectedUserIds[i];
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;

                // Kiểm tra xem người dùng có hóa đơn không bằng phương thức repository
                var hoaDonIds = await _khachHangRepository.GetHoaDonIdsByUserIdAsync(userId);
                bool hasHoaDon = hoaDonIds != null && hoaDonIds.Any();

                result.Add(new
                {
                    userId = user.Id,
                    hoTen = user.HoTen,
                    hasHoaDon
                });
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XemChiTietHoaDonTheoNguoiDung(List<string> selectedUserIds)
        {
            if (selectedUserIds == null || selectedUserIds.Count == 0)
            {
                return RedirectToAction("PhanQuyen");
            }

            string tuKhoaTimKiem = "";
            bool dauTien = true;

            // Duyệt qua từng UserId trong selectedUserIds
            for (int i = 0; i < selectedUserIds.Count; i++)
            {
                // Lấy danh sách SoHD theo UserId từ repository
                var soHDList = await _khachHangRepository.GetHoaDonIdsByUserIdAsync(selectedUserIds[i]);

                // Duyệt qua từng SoHD và thêm vào tuKhoaTimKiem
                for (int j = 0; j < soHDList.Count; j++)
                {
                    if (!dauTien)
                    {
                        tuKhoaTimKiem = tuKhoaTimKiem + ",";
                    }
                    tuKhoaTimKiem = tuKhoaTimKiem + soHDList[j].ToString();
                    dauTien = false;
                }
            }

            if (tuKhoaTimKiem == "")
            {
                return RedirectToAction("DanhSachHoaDon", "QuanLyHoaDon", new { tuKhoaTimKiem = "", chonTrangThai = "tatCa" });
            }

            return RedirectToAction("DanhSachHoaDon", "QuanLyHoaDon", new
            {
                tuKhoaTimKiem = tuKhoaTimKiem + ",",
                chonTrangThai = "tatCa"
            });
        }
    }

    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public List<string> Roles { get; set; }
        public List<string> AllRoles { get; set; }
        public bool IsLocked { get; set; }
    }

    public class AddUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<string> SelectedRoles { get; set; }
        public List<string> AllRoles { get; set; }
    }

    public class UserDetailsViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public bool IsLocked { get; set; }
        public List<string> Roles { get; set; }
        public int MaKH { get; set; }
        public string TenKH { get; set; }
        public DateTime? NgaySinhKH { get; set; }
        public string DiaChiKH { get; set; }
        public string DTKH { get; set; }
        public string EmailKH { get; set; }
        public string AvatarUrl { get; set; }
    }
}