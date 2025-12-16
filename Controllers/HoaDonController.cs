using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using System.Threading.Tasks;
using WebsiteBanHang.Repositories.EF;
using WebsiteBanHang.Controllers;
using WebsiteBanHang.Repositories.I;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Collections.Generic;
using WebsiteBanHang.Services.VnPay;
using WebsiteBanHang.Models.VnPay;
using WebsiteBanHang.Models.GioHang;
using Microsoft.AspNetCore.Identity;
using WebsiteBanHang.Repositories;
using WebsiteBanHang.Models.NguoiDung;
using Microsoft.CodeAnalysis;
using WebsiteBanHang.Services.HoaDon;
using Microsoft.AspNetCore.Authorization;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Models.QuanLyThanhToan.ViewModel;
using WebsiteBanHang.Repositories.I.QLTrangThai;
using WebsiteBanHang.Models.QuanLyTrangThai;

public class HoaDonController : Controller
{
    private readonly IVnPayService _dichVuVnPay;
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ISanPhamRepository _khoSanPham;
    private readonly IPTTTRepository _khoPhuongThucThanhToan;
    private readonly IHoaDonRepository _khoHoaDon;
    private readonly IDanhMucRepository _khoDanhMuc;
    private readonly IDSAnhRepository _khoAnhSanPham;
    private readonly IGioHangRepository _khoLuuGioHang;
    private readonly ICTGioHangRepository _khoChiTietGioHang;
    private readonly UserManager<ThongTinNguoiDung> _quanLyNguoiDung;
    private readonly IHoaDonService _hoaDonService;
    private readonly ITonKhoRepository _tonKhoRepository; // Thêm
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly ILichSuTTHDRepository _lichSuTTHDRepository;


    public HoaDonController(
        ApplicationDbContext dbContext,
        ISanPhamRepository khoSanPham,
        IDanhMucRepository khoDanhMuc,
        IDSAnhRepository khoAnhSanPham,
        IPTTTRepository khoPhuongThucThanhToan,
        IHoaDonRepository khoHoaDon,
        IVnPayService dichVuVnPay,
        IGioHangRepository khoLuuGioHang,
        ICTGioHangRepository khoChiTietGioHang,
        UserManager<ThongTinNguoiDung> quanLyNguoiDung,
        IHoaDonService hoaDonService,
        ITonKhoRepository tonKhoRepository,
        INguoiDungRepository nguoiDungRepository,
        ILichSuTTHDRepository lichSuTTHDRepository)
    {
        _dichVuVnPay = dichVuVnPay;
        _dbContext = dbContext;
        _khoSanPham = khoSanPham;
        _khoDanhMuc = khoDanhMuc;
        _khoAnhSanPham = khoAnhSanPham;
        _khoPhuongThucThanhToan = khoPhuongThucThanhToan;
        _khoHoaDon = khoHoaDon;
        _khoLuuGioHang = khoLuuGioHang;
        _khoChiTietGioHang = khoChiTietGioHang;
        _quanLyNguoiDung = quanLyNguoiDung;
        _hoaDonService = hoaDonService;
        _tonKhoRepository = tonKhoRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _lichSuTTHDRepository = lichSuTTHDRepository;

    }

    private async Task DatHoTenVaoViewBag()
    {
        if (User.Identity.IsAuthenticated)
        {
            var email = User.Identity.Name;
            var hoTen = await _nguoiDungRepository.GetHoTenByEmailAsync(email);
            ViewBag.HoTen = string.IsNullOrEmpty(hoTen) ? User.Identity.Name : hoTen;
            // Lấy AvatarUrl từ KhachHang
            var user = await _quanLyNguoiDung.GetUserAsync(User);
            if (user != null)
            {
                var khachHang = await _dbContext.KhachHang
                    .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang != null)
                {
                    ViewBag.AvatarUrl = khachHang.AvatarUrl;
                }
            }
        }
        await LaySoLuongGioHang(); // Gọi để lấy số lượng
    }


         // Hàm bất đồng bộ, không trả về giá trị (Task), chỉ lấy số lượng giỏ hàng
        private async Task LaySoLuongGioHang()
    {
        int soLuong = 0; //  số lượng mặc định là 0
        if (User.Identity.IsAuthenticated) // Kiểm tra đăng nhập chưa, nếu chưa SL=0
        {
            var user = await _quanLyNguoiDung.GetUserAsync(User); // Lấy thông tin người dùng đang đăng nhập từ Identity, await đợi kết quả từ database
            if (user != null) // Kiểm tra xem lấy thông tin người dùng, nếu không soLuong = 0
            {
                var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id); // Tìm khách hàng trong bảng KhachHangs dựa trên Id của user, await đợi kết quả
                if (khachHang != null) // Kiểm tra thấy khách hàng không, nếu không thì dừng, soLuong = 0
                {
                    var gioHang = await _dbContext.GioHang.FirstOrDefaultAsync(g => g.MaKH == khachHang.MaKH); // Tìm giỏ hàng trong bảng GioHangs dựa trên MaKH của khách hàng, await đợi kết quả
                    if (gioHang != null) // Kiểm tra có giỏ hàng không, nếu không thì dừng,soLuong = 0
                    {
                        soLuong = await _dbContext.CTGioHang // Tính tổng số lượng từ bảng CTGioHangs
                            .Where(ct => ct.MaGH == gioHang.MaGH) // Lọc các dòng có MaGH khớp với giỏ hàng của khách
                            .SumAsync(ct => ct.SoLuongThem); // Cộng tất cả SoLuongThem trong các dòng lọc được, await đợi kết quả
                    }
                }
            }
        }
        ViewBag.SoLuongGioHang = soLuong; // Gán giá trị soLuong vào ViewBag 
    }

    //---------------------------------------------------------------------------------------------------------------------------------------
    // Hành động để xem danh sách hóa đơn
    [Route("DanhSachHoaDon")]
    [Route("hoadon/DanhSachHoaDon")]
    public async Task<IActionResult> DanhSachHoaDon()
    {
        // Lấy thông tin người dùng hiện tại
        var nguoiDung = await _quanLyNguoiDung.GetUserAsync(User);
        if (nguoiDung == null)
        {
            string trangTiepTheo = "/HoaDon/DanhSachHoaDon"; // Đường dẫn đến trang danh sách hóa đơn (nơi quay lại sau khi đăng nhập)
            string trangDangNhap = "/Identity/Account/Login?returnUrl="; // Đường dẫn đến trang đăng nhập
            string duongDanDayDu = trangDangNhap + trangTiepTheo; // Sau khi đăng nhập, quay lại trang danh sách hóa đơn
            return Redirect(duongDanDayDu); // Chuyển hướng người dùng đến trang đăng nhập
        }

        // Kiểm tra nếu là admin, chuyển hướng đến trang admin
        if (await _quanLyNguoiDung.IsInRoleAsync(nguoiDung, "Admin"))
        {
            return Redirect("/Admin/QuanLyHoaDon/DanhSachHoaDon");

        }
        await DatHoTenVaoViewBag();
        // Kiểm tra xem người dùng có phải là Admin không
        bool laAdmin = await _quanLyNguoiDung.IsInRoleAsync(nguoiDung, "Admin");
        bool laEmployee = await _quanLyNguoiDung.IsInRoleAsync(nguoiDung, "Employee");
        // Lấy danh sách tất cả hóa đơn
        var danhSachHoaDon = await _khoHoaDon.GetAllAsync();
        ViewBag.IsAdmin = laAdmin;
        ViewBag.IsEmployee = laEmployee; // Truyền vai trò vào ViewBag
        // Nếu không phải Admin, chỉ lấy hóa đơn của người dùng này
        if (!laAdmin && !laEmployee)
        {
            // Lấy thông tin khách hàng của người dùng
            var khachHang = await _dbContext.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == nguoiDung.Id);
            if (khachHang != null)
            {
                // Lọc danh sách hóa đơn chỉ giữ lại hóa đơn của khách hàng này
                danhSachHoaDon = danhSachHoaDon.Where(h => h.MaKH == khachHang.MaKH).ToList();
            }
            else
            {
                // Nếu không tìm thấy khách hàng, trả về danh sách rỗng
                danhSachHoaDon = new List<HoaDon>();
            }
        }

        // Trả về view với danh sách hóa đơn đã lọc
        return View(danhSachHoaDon);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------
    
    // Hành động để xem chi tiết hóa đơn
    [Route("ChiTietHoaDon/{id}")]
    public async Task<IActionResult> ChiTietHoaDon(int id)
    {
        // Lấy thông tin hóa đơn theo ID
        var hoaDon = await _khoHoaDon.GetByIdAsync(id);
        if (hoaDon == null)
        {
            return NotFound(); // Trả về lỗi 404 nếu không tìm thấy hóa đơn
        }

        // Lấy thông tin người dùng hiện tại
        var nguoiDung = await _quanLyNguoiDung.GetUserAsync(User);
        if (nguoiDung == null)
        {
            string trangTiepTheo = "/HoaDon/ChiTietHoaDon/" + id; // Đường dẫn đến trang chi tiết hóa đơn
            string trangDangNhap = "/Identity/Account/Login?returnUrl="; // Đường dẫn đến trang đăng nhập
            string duongDanDayDu = trangDangNhap + trangTiepTheo; // Sau khi đăng nhập, quay lại trang chi tiết hóa đơn
            return Redirect(duongDanDayDu); // Chuyển hướng người dùng đến trang đăng nhập
        }

        await DatHoTenVaoViewBag();

        // Kiểm tra xem người dùng có phải là Admin hay employee không
        bool laAdmin = await _quanLyNguoiDung.IsInRoleAsync(nguoiDung, "Admin");
        bool laEmployee = await _quanLyNguoiDung.IsInRoleAsync(nguoiDung, "Employee");

        // Lấy thông tin khách hàng của người dùng
        var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == nguoiDung.Id);
        if (!laAdmin)
        {
            if (laEmployee == true)
            {
                ViewBag.CoQuyenChinhSua = true;
                ViewBag.TrangThaiList = await _dbContext.TrangThai.ToListAsync();
            }
            else
            {
                if (khachHang == null || hoaDon.MaKH != khachHang.MaKH)
                {
                    return Forbid(); // Trả về lỗi 403 nếu người dùng không có quyền xem hóa đơn này
                }
                ViewBag.NguoiMua = true;
            }
             
        }
        else
        {
            //nếu admin ko có trong khách hàng hoặc makh của admin != mã khách hàng trong hoá đơn
            if (khachHang == null || hoaDon.MaKH != khachHang.MaKH)
            {
                ViewBag.NguoiMua = false; //không hiển thị mua ngay

            }
            else
            {
                
                ViewBag.NguoiMua = true; //Hiển thị mua ngay
            }
            if (laEmployee == true)
            {
                ViewBag.CoQuyenChinhSua = true;
                ViewBag.TrangThaiList = await _dbContext.TrangThai.ToListAsync();

            }

        }

        // Trả về view với thông tin chi tiết hóa đơn
        return View(hoaDon);
    }

    // Hành động cập nhật trạng thái (chỉ dành cho Admin/Employee)
    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    [Route("CapNhatTrangThai")]
    public async Task<IActionResult> CapNhatTrangThai(int id, int maTT)
    {
        var hoaDon = await _khoHoaDon.GetByIdAsync(id);
        if (hoaDon == null)
        {
            return NotFound();
        }

        // Cập nhật trạng thái
        var lichSu = new LichSuTTHD
        {
            SoHD = id,
            MaTT = maTT,
            ThoiGianThayDoi = DateTime.Now,
            GhiChu = "Cập nhật trạng thái bởi Admin/Employee"
        };
        await _lichSuTTHDRepository.AddAsync(lichSu);

        return RedirectToAction("ChiTietHoaDon", new { id = id });
    }

    // Action xử lý chuyển hướng để chỉnh sửa thông tin giao hàng
    [Authorize] // Yêu cầu đăng nhập
    [Route("ChinhSuaThongTinGiaoHang/{id}")]
    public async Task<IActionResult> ChinhSuaThongTinGiaoHang(int id)
    {
        // Lấy thông tin hóa đơn theo ID
        var hoaDon = await _khoHoaDon.GetByIdAsync(id);
        if (hoaDon == null)
        {
            return NotFound(); // Trả về lỗi 404 nếu không tìm thấy hóa đơn
        }

        // Lấy thông tin người dùng hiện tại
        var nguoiDung = await _quanLyNguoiDung.GetUserAsync(User);
        if (nguoiDung == null)
        {
            string luuTrangHienTai = $"/HoaDon/ChiTietHoaDon/{id}";
            string trangDangNhap = "/Identity/Account/Login?returnUrl=";
            string duongDanKetHop = trangDangNhap + luuTrangHienTai;
            return Redirect(duongDanKetHop);
        }

        // Lấy thông tin khách hàng của người dùng hiện tại
        var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == nguoiDung.Id);
        if (khachHang == null || hoaDon.MaKH != khachHang.MaKH)
        {
            // Nếu không phải người mua, quay lại trang chi tiết hóa đơn
            return RedirectToAction("ChiTietHoaDon", new { id = id });
        }

        // Kiểm tra điều kiện chỉnh sửa
        var latestStatus = await _lichSuTTHDRepository.GetLatestBySoHDAsync(id);
        if (latestStatus.MaTT != 1 || hoaDon.SoLanDoiTT != 0)
        {
            return RedirectToAction("ChiTietHoaDon", new { id = id });
        }

        // Xây dựng URL chuyển hướng tới QuanLyTaiKhoan
        string trangTiepTheo = $"/HoaDon/ChiTietHoaDon/{id}";
        string trangQuanLyTaiKhoan = "/Identity/CustomAccount/QuanLyTaiKhoan?returnUrl=";
        string duongDanDayDu = trangQuanLyTaiKhoan + trangTiepTheo;
        return Redirect(duongDanDayDu);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> PaymentCallbackVnpay()
    {
        // Xử lý phản hồi từ VnPay
        var phanHoi = _dichVuVnPay.PaymentExecute(Request.Query);

        // Kiểm tra và cập nhật trạng thái hóa đơn
        if (int.TryParse(phanHoi.OrderId, out int maHoaDon)) // Chuyển OrderId thành số hóa đơn
        {
            var hoaDon = await _dbContext.HoaDon.FindAsync(maHoaDon);
            if (hoaDon != null)
            {
                if (phanHoi.Success && phanHoi.VnPayResponseCode == "00")
                {
                    // Thanh toán thành công
                    var lichSu = new LichSuTTHD
                    {
                        SoHD = maHoaDon,
                        MaTT = 1,
                        ThoiGianThayDoi = DateTime.Now,
                        GhiChu ="Thanh toán thành công qua VnPay"
                    };
                    await _lichSuTTHDRepository.AddAsync(lichSu);
                }
                else
                {
                    // Thanh toán thất bại
                    var lichSu = new LichSuTTHD
                    {
                        SoHD = maHoaDon,
                        MaTT = 9,
                        ThoiGianThayDoi = DateTime.Now,
                        GhiChu = "Thanh toán thất bại qua VnPay"
                    };
                    await _lichSuTTHDRepository.AddAsync(lichSu);
                }
            }
            else
            {
                Console.WriteLine($"Lỗi: Không tìm thấy hóa đơn với SoHD '{maHoaDon}'");
            }
        }
        else
        {
            Console.WriteLine($"Lỗi: Không thể chuyển OrderId '{phanHoi.OrderId}' thành số nguyên");
        }

        // Trả về view hiển thị kết quả thanh toán
        return View("PaymentResult", phanHoi);
    }



    [Authorize(Roles = SD.Role_Customer)]
    [HttpGet]
    // Action để hiển thị trang thanh toán chung
    public async Task<IActionResult> ThanhToanChung(int? maSP)
    {
        if (!User.Identity.IsAuthenticated)
        {
            string trangTiepTheo = maSP.HasValue
                ? $"/HoaDon/ThanhToanChung?productId={maSP}"
                : "/HoaDon/ThanhToanChung";
            string trangDangNhap = "/Identity/Account/Login?returnUrl=";
            string duongDanDayDu = trangDangNhap + trangTiepTheo;
            return Redirect(duongDanDayDu);
        }

       

        // Lấy thông tin khách hàng để lấy địa chỉ và số điện thoại
        var user = await _quanLyNguoiDung.GetUserAsync(User);
        var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
        if (khachHang == null)
        {
           
            //TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
            return RedirectToAction("Index", "ShoppingCart");
        }

         List<CartItem> danhSachSanPhamDaChon;//ĐÂY LÀ MỤC ĐÍCH SAU CÙNG,
                                             //TẤT CẢ CÁC BƯỚC TỎNG CODE NÀY CHỈ LÀ
                                             //ĐỂ ĐƯA VÀO TRANG THANHTOANCHUNG.

        if (maSP != null || maSP > 0)//Nếu có productid Tức ấn "mua ngay"
        {
            //var sanPham = _dbContext.Products.FirstOrDefault(p => p.Id == productId);// thì tìm sản phẩm trong CSDL 
            var sanPham = await _khoSanPham.GetByIdAsync(maSP.Value);// thì tìm ĐỐI TƯỢNG sản phẩm trong CSDL thông qua
                                                                          // repository thay vì truy vấn trực tiếp từ DB
            if (sanPham == null )//Khi không tìm được SanPham
            {
                return NotFound(); //trả về lỗi 404
            } //ngược lại bỏ qua đi đến đây tiếp tục làm việc   
               
            danhSachSanPhamDaChon = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = sanPham.MaSP,
                    Name = sanPham.TenSP,
                    Price = sanPham.Gia,
                    Quantity = 1,
                    ImageUrl = sanPham.UrlAnh ?? (sanPham.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/200x200"),
                    Description = sanPham.MoTa
                }
            };

            // Lưu vào Session ngay từ lần đầu vào trang
            HttpContext.Session.SetString("SelectedCartItems", JsonConvert.SerializeObject(danhSachSanPhamDaChon));
        }
        else//Nếu không có product id tức vào đây từ "giỏ hàng"
        {

            
            // Lấy dữ liệu từ Session
            var chuoiJsonSanPhamDaChon = HttpContext.Session.GetString("SelectedCartItems");
            if (chuoiJsonSanPhamDaChon == null || chuoiJsonSanPhamDaChon =="")
            {
                return RedirectToAction("Index", "ShoppingCart"); //thì chuyển về giỏ hàng
            }//ngược lại nếu ra ngoài đây thì tiếp tục bước sau

            danhSachSanPhamDaChon = 
                JsonConvert.DeserializeObject<List<CartItem>>(chuoiJsonSanPhamDaChon); //Chuyển chuỗi JSON từ Session thành danh sách các đói tượng sản phẩm CartItem
            var danhSachMaSanPham = danhSachSanPhamDaChon.Select(i => i.ProductId).ToList();//Với mỗi SP trong danhSachSanPhamDaChon gọi là i, lấy mã sản phẩm (ProductId) của nó. trước khi hỏi CSDL (không thể dùng repository vì không thao tác trên CSDL)
            //var danhSachSanPham = _dbContext.Products.Where(p=>danhSachMaSanPham.Contains(p.Id)).ToList();// với mỗi sp gọi là p  kiểm tra xem id của sp p có nằm trong danhsachmasanpham không 
            var danhSachSanPham = await _khoSanPham.GetByIdsAsync(danhSachMaSanPham);//sử dung Repository để lấy danh sách sản phẩm theo mã sản phẩm


            

            foreach (var sanPhamDaChon in danhSachSanPhamDaChon)//Với mỗi SP trong giỏ(danhSachSanPhamDaChon) gọi với tên tạm là sanPhamDaChon
            {
                var sanPham = danhSachSanPham.FirstOrDefault(p => p.MaSP == sanPhamDaChon.ProductId);//với mỗi SP gọi là p trong danhSachSanPham, kiểm tra  id của nó  == ProductId của sp trong sanPhamDaChon
                if (sanPham != null)//nếu tìm thấy sản phẩm
                {// cập nhật thông tin mới cho sản phẩm trong giỏ
                    sanPhamDaChon.ImageUrl = sanPham.UrlAnh ?? (sanPham.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/200x200");
                    sanPhamDaChon.Description = sanPham.MoTa;
                    sanPhamDaChon.Price = sanPham.Gia;
                }
                else
                {
                    TempData["Error"] = $"Sản phẩm {sanPhamDaChon.Name} không còn tồn tại.";
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }
            // Cập nhật Session với thông tin mới
            HttpContext.Session.SetString("SelectedCartItems", JsonConvert.SerializeObject(danhSachSanPhamDaChon));
        }

        // Rút gọn mô tả ngay khi tạo danh sách
        foreach (var item in danhSachSanPhamDaChon)
        {
            if (!string.IsNullOrEmpty(item.Description))
            {
                var tu = item.Description.Split(' ').Where(w => w.Length > 0).ToArray();
                if (tu.Length > 27)
                    item.Description = string.Join(" ", tu.Take(27)) + "…";
            }
        }

        var model = new ThanhToanViewModel //Tạo một đối tượng chứa dữ liệu gửi sang giao diện.
        {
            DanhSachSanPhamDaChon = danhSachSanPhamDaChon,
            DanhSachPhuongThucThanhToan = _dbContext.PTTT.ToList(),
            DiaChiGiaoHang = khachHang.DiaChiKH, // Thêm địa chỉ
            SoDienThoai = khachHang.DTKH         // Thêm số điện thoại
        };

        return View(model);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------

    [HttpPost]
    public IActionResult CapNhatTongTienSanPham(int maSanPham, int soLuong)
    {
      
        string chuoiJson = HttpContext.Session.GetString("SelectedCartItems");// Lấy chuỗi JSON từ Session
        if (chuoiJson == null || chuoiJson == "")// Kiểm tra nếu không có dữ liệu
        {
            return Json(new { thanhCong = false, tongTienSanPham = 0 });
        }
        List<CartItem> danhSach = 
            JsonConvert.DeserializeObject<List<CartItem>>(chuoiJson); // Chuyển JSON thành danh sách

        long tongTienSanPham = 0; // Biến để lưu tổng tiền
        bool timThay = false;

        for (int i = 0; i < danhSach.Count; i++)// Dùng vòng lặp for để tìm sản phẩm
        {
            if (danhSach[i].ProductId == maSanPham)
            {
                if (soLuong < 1) // Cập nhật số lượng (nếu < 1 thì đặt thành 1)
                {
                    danhSach[i].Quantity = 1;
                }
                else
                {
                    danhSach[i].Quantity = soLuong;
                }
                tongTienSanPham = (long)danhSach[i].Price * danhSach[i].Quantity;// Tính tổng tiền
                timThay = true;
                break; // Thoát vòng lặp khi tìm thấy
            }
        }

        if (timThay== true) // Kiểm tra nếu tìm thấy sản phẩm
        {
            // Lưu lại danh sách vào Session
            HttpContext.Session.SetString("SelectedCartItems", JsonConvert.SerializeObject(danhSach));
            return Json(new { thanhCong = true, tongTienSanPham });
        }
        else
        {
            return Json(new { thanhCong = false, tongTienSanPham = 0 });
        }
    }


    [HttpPost]
    public IActionResult TinhTongTienChung()
    {
        string chuoiJson = 
            HttpContext.Session.GetString("SelectedCartItems"); // Lấy chuỗi JSON từ Session
        if (chuoiJson == null || chuoiJson == "") // Kiểm tra nếu không có dữ liệu
        {
            return Json(new { thanhCong = false, tongTienChung = 0 });
        }
        List<CartItem> danhSach = 
            JsonConvert.DeserializeObject<List<CartItem>>(chuoiJson); // Chuyển JSON thành danh sách

        long tongTienChung = 0;// Biến để lưu tổng tiền chung
        for (int i = 0; i < danhSach.Count; i++)// Dùng vòng lặp for để tính tổng
        {
            tongTienChung =tongTienChung + (long)(danhSach[i].Price * danhSach[i].Quantity);
        }
        return Json(new { thanhCong = true, tongTienChung });// Trả về kết quả
    }


    /*------------------------------------------------------------------------------------------------------------------------------------------*/
    // Action xử lý thanh toán chung
    [Authorize(Roles = SD.Role_Customer)]
    [HttpPost]
    public async Task<IActionResult> XacNhanThanhToanChung(int paymentMethodId, [FromForm] Dictionary<int, int> quantities)
    {
        var nguoiDung = await _quanLyNguoiDung.GetUserAsync(User);
        var khachHang = await _dbContext.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == nguoiDung.Id);

        var chuoiJsonSanPhamDaChon = HttpContext.Session.GetString("SelectedCartItems");
        List<CartItem> danhSachSanPhamDaChon;
        string[] mangLoi = new string[6];
        int soLuongLoiViTriLoi = 0;

        if (string.IsNullOrEmpty(chuoiJsonSanPhamDaChon))
        {
            mangLoi[soLuongLoiViTriLoi] = "Không có sản phẩm nào để thanh toán.";
            soLuongLoiViTriLoi++;
            TempData["Errors"] = JsonConvert.SerializeObject(mangLoi.Take(soLuongLoiViTriLoi).ToArray());
            return RedirectToAction("ThanhToanChung");
        }
        danhSachSanPhamDaChon = JsonConvert.DeserializeObject<List<CartItem>>(chuoiJsonSanPhamDaChon);

        if (chuoiJsonSanPhamDaChon == null)
        {
            var maProductIdDauTien = quantities.Keys.FirstOrDefault();
            if (maProductIdDauTien == 0 || !quantities.ContainsKey(maProductIdDauTien))
            {
                mangLoi[soLuongLoiViTriLoi] = "Không có sản phẩm nào để thanh toán.";
                soLuongLoiViTriLoi++;
                TempData["Errors"] = JsonConvert.SerializeObject(mangLoi.Take(soLuongLoiViTriLoi).ToArray());
                return RedirectToAction("ThanhToanChung");
            }

            var sanPham = await _dbContext.SanPham.FindAsync(maProductIdDauTien);
            if (sanPham == null || quantities[maProductIdDauTien] <= 0)
            {
                mangLoi[soLuongLoiViTriLoi] = "Sản phẩm không tồn tại hoặc số lượng không hợp lệ.";
                soLuongLoiViTriLoi++;
                TempData["Errors"] = JsonConvert.SerializeObject(mangLoi.Take(soLuongLoiViTriLoi).ToArray());
                return RedirectToAction("ThanhToanChung");
            }

            danhSachSanPhamDaChon = new List<CartItem>
        {
            new CartItem
            {
                ProductId = sanPham.MaSP,
                Name = sanPham.TenSP,
                Price = sanPham.Gia,
                Quantity = quantities[maProductIdDauTien],
                ImageUrl = sanPham.UrlAnh ?? (sanPham.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/200x200"),
                Description = sanPham.MoTa
            }
        };
        }
        else
        {
            danhSachSanPhamDaChon = JsonConvert.DeserializeObject<List<CartItem>>(chuoiJsonSanPhamDaChon);
            foreach (var sanPhamDaChon in danhSachSanPhamDaChon)
            {
                if (quantities.ContainsKey(sanPhamDaChon.ProductId))
                {
                    sanPhamDaChon.Quantity = quantities[sanPhamDaChon.ProductId];
                }
            }
            HttpContext.Session.SetString("SelectedCartItems", JsonConvert.SerializeObject(danhSachSanPhamDaChon));
        }

        var danhSachMaSanPham = danhSachSanPhamDaChon.Select(i => i.ProductId).ToList();
        var danhSachSanPham = await _dbContext.SanPham
            .Where(p => danhSachMaSanPham.Contains(p.MaSP))
            .Select(p => new { p.MaSP, p.Gia })
            .ToListAsync();

        // Lấy danh sách MaSP và Quantity từ danhSachSanPhamDaChon
        var productRequirements = danhSachSanPhamDaChon
            .Select(sp => new { MaSP = sp.ProductId, Quantity = sp.Quantity })
            .ToList();

        // Bước 1: Tìm kho chung chứa tất cả sản phẩm với SLTon đủ
        var tonKhoList = await _tonKhoRepository.GetAllAsync();
        var khoChung = tonKhoList
            .GroupBy(tk => tk.MaKho)
            .Where(g => productRequirements.All(req =>
                g.Any(tk => tk.MaSP == req.MaSP && tk.SLTon >= req.Quantity)))
            .Select(g => g.Key)
            .FirstOrDefault();

        // Kiểm tra tồn kho và giá
        foreach (var sanPhamDaChon in danhSachSanPhamDaChon)
        {
            var sanPham = danhSachSanPham.FirstOrDefault(p => p.MaSP == sanPhamDaChon.ProductId);
            if (sanPham == null)
            {
                mangLoi[soLuongLoiViTriLoi] = $"Sản phẩm {sanPhamDaChon.Name} không tồn tại.";
                soLuongLoiViTriLoi++;
                continue;
            }

            // Kiểm tra số lượng tồn kho trong TonKho
            var tonKho = khoChung != 0
                ? tonKhoList.FirstOrDefault(tk => tk.MaKho == khoChung && tk.MaSP == sanPhamDaChon.ProductId && tk.SLTon >= sanPhamDaChon.Quantity)
                : null;

            if (tonKho == null)
            {
                tonKho = await _tonKhoRepository.FindKhoWithEnoughStockAsync(sanPhamDaChon.ProductId, sanPhamDaChon.Quantity);
            }

            if (tonKho == null)
            {
                mangLoi[soLuongLoiViTriLoi] = $"Sản phẩm {sanPhamDaChon.Name} không có đủ số lượng tồn kho.";
                soLuongLoiViTriLoi++;
            }

            if ((long)sanPham.Gia != sanPhamDaChon.Price)
            {
                mangLoi[soLuongLoiViTriLoi] = $"Giá sản phẩm {sanPhamDaChon.Name} đã thay đổi. Giá hiện tại: {sanPham.Gia.ToString("N0")} VND.";
                soLuongLoiViTriLoi++;
            }
        }

        // Kiểm tra địa chỉ và số điện thoại
        if (khachHang.DiaChiKH == null || khachHang.DiaChiKH.Trim() == "")
        {
            mangLoi[soLuongLoiViTriLoi] = "Vui lòng cung cấp địa chỉ giao hàng.";
            soLuongLoiViTriLoi++;
        }

        if (khachHang.DTKH == null || khachHang.DTKH.Trim() == "")
        {
            mangLoi[soLuongLoiViTriLoi] = "Vui lòng cung cấp số điện thoại liên lạc.";
            soLuongLoiViTriLoi++;
        }

        if (soLuongLoiViTriLoi > 0)
        {
            string[] loiThucTe = new string[soLuongLoiViTriLoi];
            for (int i = 0; i < loiThucTe.Length; i++)
            {
                loiThucTe[i] = mangLoi[i];
            }
            TempData["Errors"] = JsonConvert.SerializeObject(loiThucTe);
            return RedirectToAction("ThanhToanChung");
        }

        if (khachHang == null)
        {
            mangLoi[soLuongLoiViTriLoi] = "Không tìm thấy thông tin khách hàng.";
            soLuongLoiViTriLoi++;
            TempData["Errors"] = JsonConvert.SerializeObject(new[] { mangLoi[soLuongLoiViTriLoi - 1] });
            return RedirectToAction("ThanhToanChung");
        }

        var maHoaDon = await _hoaDonService.TaoHoaDonVaGiamTonKhoAsync(khachHang.MaKH, paymentMethodId, danhSachSanPhamDaChon, khachHang.DiaChiKH, khachHang.DTKH);

        if (chuoiJsonSanPhamDaChon != null && chuoiJsonSanPhamDaChon != "" && chuoiJsonSanPhamDaChon.Length > 0)
        {
            await _hoaDonService.XoaSanPhamKhoiGioHangAsync(khachHang.MaKH, danhSachMaSanPham);
        }

        if (paymentMethodId == 2)
        {
            long tongTien = 0;
            for (int i = 0; i < danhSachSanPhamDaChon.Count; i++)
            {
                tongTien += danhSachSanPhamDaChon[i].Quantity * (long)danhSachSanPhamDaChon[i].Price;
            }
            var thongTinThanhToan = new PaymentInformationModel
            {
                OrderId = maHoaDon,
                Amount = (long)tongTien,
                OrderDescription = $"Thanh toán đơn hàng {maHoaDon}",
                OrderType = "order",
                Name = danhSachSanPhamDaChon.Count == 1 ? danhSachSanPhamDaChon.First().Name : "Thanh toán nhiều sản phẩm"
            };

            var urlThanhToan = _dichVuVnPay.CreatePaymentUrl(thongTinThanhToan, HttpContext);
            HttpContext.Session.Remove("SelectedCartItems");
            return Redirect(urlThanhToan);
        }

        if (chuoiJsonSanPhamDaChon != null && chuoiJsonSanPhamDaChon.Length > 0 && chuoiJsonSanPhamDaChon != "")
        {
            HttpContext.Session.Remove("SelectedCartItems");
        }

        return RedirectToAction("DanhSachHoaDon", "HoaDon");
    }
}