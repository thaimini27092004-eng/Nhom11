
/*HomeControllter.cs*/
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi.ViewModel;
using WebsiteBanHang.Repositories.EF;
using WebsiteBanHang.Repositories.I;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using WebsiteBanHang.Repositories.I.QLDanhGiaPhanHoiTraLoi;
using WebsiteBanHang.Services.AITuVan;
using WebsiteBanHang.Repositories.I.QLKhoHang;
using WebsiteBanHang.Models.QuanLyBanner;
using WebsiteBanHang.Repositories.I.QLBanner;


namespace WebsiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISanPhamRepository _sanphamRepository;
        private readonly IPTTTRepository _pTTTRepository;
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IDanhMucRepository _danhmucRepository;
        private readonly IDSAnhRepository _dsAnhRepository;
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBinhLuanRepository _binhLuanRepository;
        private readonly IPhanHoiBinhLuanRepository _traLoiRepository;
        private readonly ILikeBinhLuanRepository _likePhanHoiRepository;
        private readonly ILikePhanHoiBinhLuanRepository _likeTraLoiRepository;
        private readonly IDanhGiaRepository _danhGiaRepository;
        private readonly ILikeDanhGiaRepository _likeDanhGiaRepository;
        private readonly IPhanHoiDanhGiaRepository _phanHoiDanhGiaRepository;
        private readonly ILikePhanHoiDanhGiaRepository _likePhanHoiDanhGiaRepository;
        private readonly ICTHDRepository _cthdRepository;
        private readonly IKhoRepository _khoRepository;
        private readonly IBannerQuangCaoRepository _bannerQuangCaoRepository;



        public HomeController(
            ISanPhamRepository sanphamRepository,
            IDanhMucRepository danhmucRepository,
            IDSAnhRepository dsAnhRepository,
            IPTTTRepository pTTTRepository,
            IHoaDonRepository hoaDonRepository,
            INguoiDungRepository nguoiDungRepository,
            UserManager<ThongTinNguoiDung> userManager,
            ApplicationDbContext context,
            IBinhLuanRepository binhLuanRepository,
            IPhanHoiBinhLuanRepository traLoiRepository,
            ILikeBinhLuanRepository likePhanHoiRepository,
            ILikePhanHoiBinhLuanRepository likeTraLoiRepository,
            IDanhGiaRepository danhGiaRepository,
            ILikeDanhGiaRepository likeDanhGiaRepository,
            IPhanHoiDanhGiaRepository phanHoiDanhGiaRepository,
            ILikePhanHoiDanhGiaRepository likePhanHoiDanhGiaRepository,
            ICTHDRepository cthdRepository,
            IKhoRepository khoRepository,
            IBannerQuangCaoRepository bannerQuangCaoRepository)
        {
            _sanphamRepository = sanphamRepository;
            _danhmucRepository = danhmucRepository;
            _dsAnhRepository = dsAnhRepository;
            _pTTTRepository = pTTTRepository;
            _hoaDonRepository = hoaDonRepository;
            _nguoiDungRepository = nguoiDungRepository;
            _userManager = userManager;
            _context = context;
            _binhLuanRepository = binhLuanRepository;
            _traLoiRepository = traLoiRepository;
            _likePhanHoiRepository = likePhanHoiRepository;
            _likeTraLoiRepository = likeTraLoiRepository;
            _danhGiaRepository = danhGiaRepository;
            _likeDanhGiaRepository = likeDanhGiaRepository;
            _phanHoiDanhGiaRepository = phanHoiDanhGiaRepository;
            _likePhanHoiDanhGiaRepository = likePhanHoiDanhGiaRepository;
            _cthdRepository = cthdRepository;
            _cthdRepository = cthdRepository;
            _khoRepository = khoRepository;
            _bannerQuangCaoRepository = bannerQuangCaoRepository;

        }

        // Hàm bất đồng bộ, không trả về giá trị (Task), chỉ lấy số lượng giỏ hàng
        private async Task LaySoLuongGioHang()
        {
            int soLuong = 0; //  số lượng mặc định là 0
            if (User.Identity.IsAuthenticated) // Kiểm tra đăng nhập chưa, nếu chưa SL=0
            {
                var user = await _userManager.GetUserAsync(User); // Lấy thông tin người dùng đang đăng nhập từ Identity, await đợi kết quả từ database
                if (user != null) // Kiểm tra xem lấy thông tin người dùng, nếu không soLuong = 0
                {
                    var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id); // Tìm khách hàng trong bảng KhachHangs dựa trên Id của user, await đợi kết quả
                    if (khachHang != null) // Kiểm tra thấy khách hàng không, nếu không thì dừng, soLuong = 0
                    {
                        var gioHang = await _context.GioHang.FirstOrDefaultAsync(g => g.MaKH == khachHang.MaKH); // Tìm giỏ hàng trong bảng GioHangs dựa trên MaKH của khách hàng, await đợi kết quả
                        if (gioHang != null) // Kiểm tra có giỏ hàng không, nếu không thì dừng,soLuong = 0
                        {
                            soLuong = await _context.CTGioHang // Tính tổng số lượng từ bảng CTGioHangs
                                .Where(ct => ct.MaGH == gioHang.MaGH) // Lọc các dòng có MaGH khớp với giỏ hàng của khách
                                .SumAsync(ct => ct.SoLuongThem); // Cộng tất cả SoLuongThem trong các dòng lọc được, await đợi kết quả
                        }
                    }
                }
            }
            ViewBag.SoLuongGioHang = soLuong; // Gán giá trị soLuong vào ViewBag 
        }


        private async Task DatHoTenVaoViewBag()
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                var hoTen = await _nguoiDungRepository.GetHoTenByEmailAsync(email);
                ViewBag.HoTen = string.IsNullOrEmpty(hoTen) ? User.Identity.Name : hoTen;
                // Lấy AvatarUrl từ KhachHang
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var khachHang = await _context.KhachHang
                        .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                    if (khachHang != null)
                    {
                        ViewBag.AvatarUrl = khachHang.AvatarUrl;
                    }
                }
            }
            await LaySoLuongGioHang(); // Gọi để lấy số lượng
        }



        // Trang chủ: Hiển thị nội dung giới thiệu và 4 sản phẩm đề xuất
        public async Task<IActionResult> Index()
        {
            await DatHoTenVaoViewBag();
            // Lấy danh sách sản phẩm đề xuất cho banner
            var sanPhamsDeXuat = await _sanphamRepository.GetAllAsync();
            sanPhamsDeXuat = sanPhamsDeXuat.Where(sp => sp.TTDeXuat).ToList();

            // Xáo trộn danh sách sản phẩm đề xuất
            var random = new Random();
            sanPhamsDeXuat = sanPhamsDeXuat.OrderBy(x => random.Next()).ToList();

            // Lấy danh sách ảnh quảng cáo từ cơ sở dữ liệu
            var bannerQuangCaos = await _bannerQuangCaoRepository.GetAllAsync();

            // Xáo trộn danh sách ảnh quảng cáo
            bannerQuangCaos = bannerQuangCaos.OrderBy(x => random.Next()).ToList();

            // Tạo ViewModel cho banner
            var bannerViewModel = new HomeBannerViewModel
            {
                SanPhamsDeXuat = sanPhamsDeXuat.ToList(),
                BannerQuangCao = bannerQuangCaos.ToList()
            };

            // Truyền ViewModel vào ViewBag
            ViewBag.BannerViewModel = bannerViewModel;

            // Lấy danh sách sản phẩm đề xuất cho phần products-section
            var sanPhams = await LayDanhSachSanPhamDeXuat();

            // Lấy danh sách danh mục đề xuất
            var danhMucDeXuat = await LayDanhSachDanhMucDeXuat();
            var danhMucSanPham = new Dictionary<DanhMuc, List<SanPham>>();
            foreach (var dm in danhMucDeXuat)
            {
                var spTrongDanhMuc = await LayDanhSachSanPhamDeXuat(dm.MaDM);
                danhMucSanPham.Add(dm, spTrongDanhMuc);
            }

            // Truyền danh sách danh mục và sản phẩm vào ViewBag
            ViewBag.DanhMucSanPham = danhMucSanPham;

            return View(sanPhams);
        }


        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> SanPhams(string tuKhoaTimKiem = "", string chonDanhMuc = "", int? chonKho = null, string sapXepGia = "macDinh")
        {
            // Lấy tất cả sản phẩm từ repository
            var tatCaSanPhamMang = (await _sanphamRepository.GetAllAsync()).ToArray();
            var danhSachSanPham = new List<SanPham>();

            // Duyệt qua tất cả sản phẩm và chỉ thêm những sản phẩm có TTHienThi = true
            for (int i = 0; i < tatCaSanPhamMang.Length; i++)
            {
                if (tatCaSanPhamMang[i].TTHienThi == true)
                {
                    danhSachSanPham.Add(tatCaSanPhamMang[i]);
                }
            }

            // Lưu các giá trị bộ lọc để hiển thị lại trên giao diện
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonDanhMuc"] = chonDanhMuc;
            ViewData["chonKho"] = chonKho;
            ViewData["sapXepGia"] = sapXepGia;

            // Lấy danh sách danh mục và kho hiển thị
            var danhMucs = (await _danhmucRepository.GetAllAsync()).Where(dm => dm.TTHienThi).ToList();
            var khos = (await _khoRepository.GetAllAsync()).Where(k => k.TTHienThi).ToList();
            ViewBag.danhmucs = danhMucs;
            ViewBag.khos = khos;

            // Lọc theo danh mục
            int maDM = 0;
            bool coTheLocDanhMuc = false;
            if (!string.IsNullOrEmpty(chonDanhMuc))
            {
                bool toanSo = true;
                for (int i = 0; i < chonDanhMuc.Length; i++)
                {
                    if (!char.IsDigit(chonDanhMuc[i]))
                    {
                        toanSo = false;
                        break;
                    }
                }
                if (toanSo)
                {
                    maDM = int.Parse(chonDanhMuc);
                    coTheLocDanhMuc = true;
                }
            }
            if (coTheLocDanhMuc)
            {
                danhSachSanPham = danhSachSanPham.Where(p => p.MaDM == maDM).ToList();
            }

            // Lọc theo kho
            if (chonKho.HasValue)
            {
                var sanPhamTheoKho = await _sanphamRepository.GetByMaKhoAsync(chonKho.Value);
                danhSachSanPham = danhSachSanPham.Where(p => sanPhamTheoKho.Any(sp => sp.MaSP == p.MaSP)).ToList();
            }

            // Sắp xếp theo giá
            if (sapXepGia == "thapDenCao")
            {
                danhSachSanPham = danhSachSanPham.OrderBy(p => p.Gia).ToList();
            }
            else if (sapXepGia == "caoDenThap")
            {
                danhSachSanPham = danhSachSanPham.OrderByDescending(p => p.Gia).ToList();
            }

            // Nếu không có từ khóa tìm kiếm, trả về danh sách đã lọc
            if (string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                await DatHoTenVaoViewBag();
                return View(danhSachSanPham);
            }

            // Tìm kiếm theo từ khóa trên danh sách đã lọc
            var mangTuKhoa = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var danhSachKetQuaTimKiem = new List<SanPham>();
            var danhSachDiemSo = new List<int>();

            for (int i = 0; i < danhSachSanPham.Count; i++)
            {
                int diem = TinhSoTuKhop(danhSachSanPham[i], mangTuKhoa);
                if (diem > 0)
                {
                    danhSachKetQuaTimKiem.Add(danhSachSanPham[i]);
                    danhSachDiemSo.Add(diem);
                }
            }

            // Sắp xếp kết quả theo điểm giảm dần
            for (int i = 0; i < danhSachKetQuaTimKiem.Count - 1; i++)
            {
                for (int j = i + 1; j < danhSachKetQuaTimKiem.Count; j++)
                {
                    if (danhSachDiemSo[i] < danhSachDiemSo[j])
                    {
                        var sanPhamTam = danhSachKetQuaTimKiem[i];
                        danhSachKetQuaTimKiem[i] = danhSachKetQuaTimKiem[j];
                        danhSachKetQuaTimKiem[j] = sanPhamTam;
                        var diemTam = danhSachDiemSo[i];
                        danhSachDiemSo[i] = danhSachDiemSo[j];
                        danhSachDiemSo[j] = diemTam;
                    }
                }
            }

            // Kiểm tra kết quả tìm kiếm
            if (danhSachKetQuaTimKiem.Count > 0)
            {
                await DatHoTenVaoViewBag();
                return View(danhSachKetQuaTimKiem);
            }
            else
            {
                ViewBag.KetQuaTimKiem = "Không có sản phẩm liên quan.";
                var danhSachGoiY = new List<SanPham>();
                for (int i = 0; i < 3 && i < danhSachSanPham.Count; i++)
                {
                    danhSachGoiY.Add(danhSachSanPham[i]);
                }
                await DatHoTenVaoViewBag();
                return View(danhSachGoiY);
            }
        }

        /*___________________________________________ Xử lý hiển thị index____________________________________________________*/
        private async Task<List<DanhMuc>> LayDanhSachDanhMucDeXuat()
        {
            var tatCaDanhMucMang = (await _danhmucRepository.GetAllAsync()).ToArray();
            var tatCaDanhMuc = new List<DanhMuc>();

            // Lọc danh mục có TTHienThi = true
            for (int i = 0; i < tatCaDanhMucMang.Length; i++)
            {
                if (tatCaDanhMucMang[i].TTHienThi == true)
                {
                    tatCaDanhMuc.Add(tatCaDanhMucMang[i]);
                }
            }

            var danhSachDanhMucDeXuat = new List<DanhMuc>();

            // Lấy các danh mục có TTDeXuat = true, tối đa 3 danh mục
            int dem = 0;
            for (int i = 0; i < tatCaDanhMuc.Count && dem < 3; i++)
            {
                if (tatCaDanhMuc[i].TTDeXuat == true)
                {
                    danhSachDanhMucDeXuat.Add(tatCaDanhMuc[i]);
                    dem++;
                }
            }

            // Nếu chưa đủ 3 danh mục, bổ sung ngẫu nhiên
            if (danhSachDanhMucDeXuat.Count < 3 && tatCaDanhMuc.Count > 0)
            {
                var soLuongConLai = 3 - danhSachDanhMucDeXuat.Count;
                var danhSachConLai = new List<DanhMuc>();

                // Lọc các danh mục chưa có trong danhSachDanhMucDeXuat
                for (int i = 0; i < tatCaDanhMuc.Count; i++)
                {
                    bool daCo = false;
                    for (int j = 0; j < danhSachDanhMucDeXuat.Count; j++)
                    {
                        if (tatCaDanhMuc[i].MaDM == danhSachDanhMucDeXuat[j].MaDM)
                        {
                            daCo = true;
                            break;
                        }
                    }
                    if (!daCo)
                    {
                        danhSachConLai.Add(tatCaDanhMuc[i]);
                    }
                }

                // Chọn ngẫu nhiên từ danhSachConLai
                Random random = new Random();
                for (int i = 0; i < soLuongConLai && danhSachConLai.Count > 0; i++)
                {
                    int chiSoNgauNhien = random.Next(0, danhSachConLai.Count);
                    danhSachDanhMucDeXuat.Add(danhSachConLai[chiSoNgauNhien]);
                    danhSachConLai.RemoveAt(chiSoNgauNhien);
                }
            }

            return danhSachDanhMucDeXuat;
        }

        private async Task<List<SanPham>> LayDanhSachSanPhamDeXuat(int? maDM = null)
        {
            var tatCaSanPhamMang = (await _sanphamRepository.GetAllAsync()).ToArray();
            var tatCaSanPham = new List<SanPham>();

            // Lọc sản phẩm có TTHienThi = true và thuộc danh mục nếu maDM được chỉ định
            for (int i = 0; i < tatCaSanPhamMang.Length; i++)
            {
                if (tatCaSanPhamMang[i].TTHienThi == true && (!maDM.HasValue || tatCaSanPhamMang[i].MaDM == maDM.Value))
                {
                    tatCaSanPham.Add(tatCaSanPhamMang[i]);
                }
            }

            var danhSachSanPhamDeXuat = new List<SanPham>();

            // Lấy các sản phẩm có TTDeXuat = true, tối đa 4 sản phẩm
            int dem = 0;
            for (int i = 0; i < tatCaSanPham.Count && dem < 4; i++)
            {
                if (tatCaSanPham[i].TTDeXuat == true)
                {
                    danhSachSanPhamDeXuat.Add(tatCaSanPham[i]);
                    dem++;
                }
            }

            // Nếu chưa đủ 4 sản phẩm, bổ sung ngẫu nhiên
            if (danhSachSanPhamDeXuat.Count < 4 && tatCaSanPham.Count > 0)
            {
                var soLuongConLai = 4 - danhSachSanPhamDeXuat.Count;
                var danhSachConLai = new List<SanPham>();

                // Lọc các sản phẩm chưa có trong danhSachSanPhamDeXuat
                for (int i = 0; i < tatCaSanPham.Count; i++)
                {
                    bool daCo = false;
                    for (int j = 0; j < danhSachSanPhamDeXuat.Count; j++)
                    {
                        if (tatCaSanPham[i].MaSP == danhSachSanPhamDeXuat[j].MaSP)
                        {
                            daCo = true;
                            break;
                        }
                    }
                    if (!daCo)
                    {
                        danhSachConLai.Add(tatCaSanPham[i]);
                    }
                }

                // Chọn ngẫu nhiên từ danhSachConLai
                Random random = new Random();
                for (int i = 0; i < soLuongConLai && danhSachConLai.Count > 0; i++)
                {
                    int chiSoNgauNhien = random.Next(0, danhSachConLai.Count);
                    danhSachSanPhamDeXuat.Add(danhSachConLai[chiSoNgauNhien]);
                    danhSachConLai.RemoveAt(chiSoNgauNhien);
                }
            }

            return danhSachSanPhamDeXuat;
        }


        /*___________________________________________ Xử lý thanh tìm kiếm____________________________________________________*/


        // Action SearchSuggestions: Trả về gợi ý thu nhỏ khi gõ ký tự
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonDanhMuc = "", int? chonKho = null, string sapXepGia = "macDinh")
        {
            // Lấy tất cả sản phẩm từ database và chuyển thành List
            var sanphams = (await _sanphamRepository.GetAllAsync()).ToList();

            // Chỉ lấy sản phẩm có TTHienThi = true
            sanphams = sanphams.Where(p => p.TTHienThi).ToList();

            // Lọc theo danh mục
            int maDM = 0;
            bool coTheLocDanhMuc = false;
            if (!string.IsNullOrEmpty(chonDanhMuc))
            {
                bool toanSo = true;
                for (int i = 0; i < chonDanhMuc.Length; i++)
                {
                    if (!char.IsDigit(chonDanhMuc[i]))
                    {
                        toanSo = false;
                        break;
                    }
                }
                if (toanSo)
                {
                    maDM = int.Parse(chonDanhMuc);
                    coTheLocDanhMuc = true;
                }
            }
            if (coTheLocDanhMuc)
            {
                sanphams = sanphams.Where(p => p.MaDM == maDM).ToList();
            }

            // Lọc theo kho
            if (chonKho.HasValue)
            {
                var sanPhamTheoKho = await _sanphamRepository.GetByMaKhoAsync(chonKho.Value);
                sanphams = sanphams.Where(p => sanPhamTheoKho.Any(sp => sp.MaSP == p.MaSP)).ToList();
            }

            // Sắp xếp theo giá
            if (sapXepGia == "thapDenCao")
            {
                sanphams = sanphams.OrderBy(p => p.Gia).ToList();
            }
            else if (sapXepGia == "caoDenThap")
            {
                sanphams = sanphams.OrderByDescending(p => p.Gia).ToList();
            }

            // Chuyển từ khóa thành chữ thường
            string tuKhoa = term.ToLower();
            var goiY = new List<object>();
            var diemSo = new List<int>();

            // Duyệt qua từng sản phẩm
            for (int i = 0; i < sanphams.Count; i++)
            {
                int diem = TinhSoKyTuKhop(sanphams[i], tuKhoa);
                if (diem > 0)
                {
                    goiY.Add(new
                    {
                        maSP = sanphams[i].MaSP,
                        tenSP = sanphams[i].TenSP,
                        urlAnh = sanphams[i].UrlAnh,
                        gia = sanphams[i].Gia
                    });
                    diemSo.Add(diem);
                }
            }

            // Sắp xếp gợi ý theo điểm giảm dần
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

            // Lấy tối đa 3 gợi ý
            var ketQua = new List<object>();
            for (int i = 0; i < 3 && i < goiY.Count; i++)
            {
                ketQua.Add(goiY[i]);
            }

            return Json(ketQua);
        }

        // Hàm tính số từ khớp (dùng cho tìm kiếm chính khi nhấn Enter)


        // Hàm tính số ký tự khớp (dùng cho gợi ý thu nhỏ)
        private int TinhSoKyTuKhop(SanPham sanPham, string tuKhoa)
        {
            // Khởi tạo biến đếm số ký tự khớp
            int soKyTuKhop = 0;
            // Lấy tên sản phẩm, chuyển thành chữ thường, nếu null thì dùng chuỗi rỗng
            var tenSanPham = sanPham.TenSP?.ToLower() ?? "";
            // Lấy mô tả sản phẩm, chuyển thành chữ thường, nếu null thì dùng chuỗi rỗng
            var moTaSanPham = sanPham.MoTa?.ToLower() ?? "";
            // Chia từ khóa thành các từ riêng lẻ
            var tuKhoaArray = tuKhoa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Duyệt qua từng từ trong từ khóa
            foreach (var tu in tuKhoaArray)
            {
                // Nếu từ nằm trong tên sản phẩm
                if (tenSanPham.Contains(tu))
                    soKyTuKhop += tu.Length * 2; // Tên khớp, cộng điểm gấp đôi độ dài từ
                // Nếu từ nằm trong mô tả sản phẩm
                else if (moTaSanPham.Contains(tu))
                    soKyTuKhop += tu.Length; // Mô tả khớp, cộng điểm bằng độ dài từ
            }
            // Trả về tổng điểm
            return soKyTuKhop;
        }
        private int TinhSoTuKhop(SanPham sanPham, string[] mangTuKhoa)
        {
            int soTuKhop = 0;
            var tenSanPham = (" " + (sanPham.TenSP?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var moTaSanPham = (" " + (sanPham.MoTa?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuKhoa in mangTuKhoa)
            {
                if (tenSanPham.Contains(tuKhoa))
                    soTuKhop += 2;
                else if (moTaSanPham.Contains(tuKhoa))
                    soTuKhop += 1;
            }
            return soTuKhop;
        }

        /*___________________________________________ hiển thị trang chi tiết sản phẩm____________________________________________________*/
        // Cập nhật ChiTietSanPham để lấy danh sách bình luận
        public async Task<IActionResult> ChiTietSanPham(int id, int pageNumber = 1)
        {
            var sanPham = await _sanphamRepository.GetByIdAsync(id);
            if (sanPham == null || sanPham.TTHienThi == false)
            {
                return NotFound();
            }
            const int kichThuocTrang = 5; // Hiển thị 5 bình luận mỗi trang
            var phanHoiList = await _binhLuanRepository.GetByMaSPPagedAsync(id, pageNumber, kichThuocTrang);
            var soLuongTraLoi = await GetSoLuongTraLoiAsync(phanHoiList);
            // Tạo danh sách PhanHoiViewModel
            var phanHoiViewModels =  await TaoPhanHoiViewModelsAsync(phanHoiList, soLuongTraLoi);

            // Tính tổng số trang
            var tongSoPhanHoi = await _context.BinhLuan.CountAsync(ph => ph.MaSP == id);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / kichThuocTrang);

            // Cập nhật thông tin phân trang cho ViewModel
            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = pageNumber;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = kichThuocTrang;
            }


            var danhGiaList = await _danhGiaRepository.GetByMaSPPagedAsync(id, pageNumber, kichThuocTrang);
            var soLuongPhanHoiDanhGia = await GetSoLuongPhanHoiDanhGiaAsync(danhGiaList);
            var danhGiaViewModels = await TaoDanhGiaViewModelsAsync(danhGiaList, soLuongPhanHoiDanhGia);

            var tongSoDanhGia = await _context.DanhGia.CountAsync(dg => dg.MaSP == id && dg.TTHienThi == true);
            var tongSoTrangDanhGia = (int)Math.Ceiling((double)tongSoDanhGia / kichThuocTrang);
            foreach (var viewModel in danhGiaViewModels)
            {
                viewModel.TrangHienTai = pageNumber;
                viewModel.TongSoTrang = tongSoTrangDanhGia;
                viewModel.KichThuocTrang = kichThuocTrang;
            }

            bool daMuaSanPham = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                ViewData["KhachHang"] = khachHang;
                if (khachHang != null)
                {
                    daMuaSanPham = await _context.CTHD
                        .AnyAsync(ct => ct.SoHD != null && ct.HoaDon.MaKH == khachHang.MaKH && ct.MaSP == id);
                }
            }
            ViewData["PhanHoiList"] = phanHoiViewModels;

    

            var tongLuotThich = await _context.LikeBinhLuan
                .Where(lph => _context.BinhLuan.Any(ph => ph.MaBL == lph.MaBL && ph.MaSP == id))
                .CountAsync();
            var trungBinhLuotThich = tongSoPhanHoi > 0 ? Math.Round((double)tongLuotThich / tongSoPhanHoi, 1) : 0;
            ViewData["TongSoPhanHoi"] = tongSoPhanHoi;
            ViewData["TrungBinhLuotThich"] = trungBinhLuotThich;
            ViewData["DaMuaSanPham"] = daMuaSanPham;
            ViewData["DanhGiaList"] = danhGiaViewModels;
            await DatHoTenVaoViewBag();
            return View(sanPham);
        }

        /*________________________________________________Xử lý phản hồi và trả lời phản hồi (bình luận)_____________________________________________________*/
        
        private async Task<Dictionary<int, int>> GetSoLuongTraLoiAsync(IEnumerable<BinhLuan> phanHoiList)
        {
            var soLuongTraLoi = new Dictionary<int, int>();
            foreach (var ph in phanHoiList)
            {
                var traLoiList = await _traLoiRepository.GetByMaPHAsync(ph.MaBL);
                soLuongTraLoi[ph.MaBL] = traLoiList.Count();
            }
            return soLuongTraLoi;
        }

        private async Task<Dictionary<int, int>> GetSoLuongPhanHoiDanhGiaAsync(IEnumerable<DanhGia> danhGiaList)
        {
            var soLuongPhanHoiDanhGia = new Dictionary<int, int>();
            foreach (var dg in danhGiaList)
            {
                var phanHoiList = await _phanHoiDanhGiaRepository.GetByMaDGPagedAsync(dg.MaDG, 1, int.MaxValue);
                soLuongPhanHoiDanhGia[dg.MaDG] = phanHoiList.Count();
            }
            return soLuongPhanHoiDanhGia;
        }


        /*----------------------------------------------------------------------------------------------------------------------------------------------------------*/
        private async Task<List<DanhGiaViewModel>> TaoDanhGiaViewModelsAsync(IEnumerable<DanhGia> danhGiaList, Dictionary<int, int> soLuongPhanHoiDanhGia)
        {
            var danhGiaViewModels = new List<DanhGiaViewModel>();
            int maKH = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang != null)
                {
                    maKH = khachHang.MaKH;
                }
            }

            foreach (var danhGia in danhGiaList)
            {
                var daMua = await _context.CTHD
                    .AnyAsync(ct => ct.SoHD != null && ct.HoaDon.MaKH == danhGia.CTHD.HoaDon.MaKH && ct.MaSP == danhGia.MaSP);
                var viewModel = new DanhGiaViewModel
                {
                    DanhGia = danhGia,
                    SoLuongPhanHoiDanhGia = soLuongPhanHoiDanhGia.ContainsKey(danhGia.MaDG) ? soLuongPhanHoiDanhGia[danhGia.MaDG] : 0,
                    DaMuaSanPham = daMua,
                    DaThich = maKH != 0 && await _likeDanhGiaRepository.ExistsAsync(maKH, danhGia.MaDG),
                    SoLuotThich = (await _likeDanhGiaRepository.GetByMaDGAsync(danhGia.MaDG)).Count()
                };
                danhGiaViewModels.Add(viewModel);
            }
            return danhGiaViewModels;
        }

        private async Task<List<PhanHoiDanhGiaViewModel>> TaoPhanHoiDanhGiaViewModelsAsync(IEnumerable<PhanHoiDanhGia> phanHoiList)
        {
            var phanHoiViewModels = new List<PhanHoiDanhGiaViewModel>();
            int maKH = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang != null)
                {
                    maKH = khachHang.MaKH;
                }
            }

            foreach (var phanHoi in phanHoiList)
            {
                var danhGia = await _danhGiaRepository.GetByIdAsync(phanHoi.MaDG);
                var daMua = await _context.CTHD
                    .AnyAsync(ct => ct.SoHD != null && ct.HoaDon.MaKH == phanHoi.MaKH && ct.MaSP == danhGia.MaSP);

                // Chuyển đổi @KHx thành @TênKháchHàng
                string noiDungHienThi = await ConvertKHxToTenKHAsync(phanHoi.NoiDungPHDG);

                var viewModel = new PhanHoiDanhGiaViewModel
                {
                    PhanHoiDanhGia = phanHoi,
                    DaMuaSanPham = daMua,
                    NoiDungHienThi = noiDungHienThi,
                    DaThich = maKH != 0 && await _likePhanHoiDanhGiaRepository.ExistsAsync(maKH, phanHoi.MaPHDG),
                    SoLuotThich = (await _likePhanHoiDanhGiaRepository.GetByMaPHDGAsync(phanHoi.MaPHDG)).Count()
                };
                phanHoiViewModels.Add(viewModel);
            }
            return phanHoiViewModels;
        }

        private async Task<List<PhanHoiViewModel>> TaoPhanHoiViewModelsAsync(IEnumerable<BinhLuan> phanHoiList, Dictionary<int, int> soLuongTraLoi)
        {
            var phanHoiViewModels = new List<PhanHoiViewModel>();
            int maKH = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang != null)
                {
                    maKH = khachHang.MaKH;
                }
            }

            foreach (var phanHoi in phanHoiList)
            {
                var daMua = await _context.CTHD
                            .AnyAsync(ct => ct.SoHD != null && ct.HoaDon.MaKH == phanHoi.MaKH && ct.MaSP == phanHoi.MaSP);
                var viewModel = new PhanHoiViewModel
                {
                    BinhLuan = phanHoi,
                    SoLuongTraLoi = soLuongTraLoi.ContainsKey(phanHoi.MaBL) ? soLuongTraLoi[phanHoi.MaBL] : 0,
                    SoLuotThich = (await _likePhanHoiRepository.GetByMaPHAsync(phanHoi.MaBL)).Count(),
                    DaThich = maKH != 0 && await _likePhanHoiRepository.ExistsAsync(maKH, phanHoi.MaBL),
                    DaMuaSanPham = daMua
                };
                phanHoiViewModels.Add(viewModel);
            }
            // Sắp xếp theo SoLuotThich giảm dần, sau đó theo NgayPH giảm dần
            return phanHoiViewModels;
        }

        private async Task<List<TraLoiViewModel>> TaoTraLoiViewModelsAsync(IEnumerable<PhanHoiBinhLuan> traLoiList)
        {
            var traLoiViewModels = new List<TraLoiViewModel>();
            int maKH = 0;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang != null)
                {
                    maKH = khachHang.MaKH;
                }
            }

            foreach (var traLoi in traLoiList)
            {
                var phanHoi = await _binhLuanRepository.GetByIdAsync(traLoi.MaBL);
                var daMua = await _context.CTHD
                    .AnyAsync(ct => ct.SoHD != null && ct.HoaDon.MaKH == traLoi.MaKH && ct.MaSP == phanHoi.MaSP);

                // Chuyển đổi @KHx thành @TênKháchHàng
                string noiDungHienThi = await ConvertKHxToTenKHAsync(traLoi.NoiDungPHBL);

                var viewModel = new TraLoiViewModel
                {
                    TraLoi = traLoi,
                    SoLuotThich = (await _likeTraLoiRepository.GetByMaTLAsync(traLoi.MaPHBL)).Count(),
                    DaThich = maKH != 0 && await _likeTraLoiRepository.ExistsAsync(maKH, traLoi.MaPHBL),
                    DaMuaSanPham = daMua,
                    NoiDungHienThi = noiDungHienThi
                };
                traLoiViewModels.Add(viewModel);
            }
            return traLoiViewModels;
        }



        /*-----------------------------------------------------------------------------------*/
        [HttpGet]
        public async Task<IActionResult> GetDanhGiaByMaSP(int maSP, int pageNumber = 1, int pageSize = 5)
        {
            var danhGiaList = await _danhGiaRepository.GetByMaSPPagedAsync(maSP, pageNumber, pageSize);
            var soLuongPhanHoiDanhGia = await GetSoLuongPhanHoiDanhGiaAsync(danhGiaList);
            var danhGiaViewModels = await TaoDanhGiaViewModelsAsync(danhGiaList, soLuongPhanHoiDanhGia);

            var tongSoDanhGia = await _context.DanhGia.CountAsync(dg => dg.MaSP == maSP && dg.TTHienThi == true);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoDanhGia / pageSize);

            foreach (var viewModel in danhGiaViewModels)
            {
                viewModel.TrangHienTai = pageNumber;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                ViewData["KhachHang"] = khachHang;
            }

            return PartialView("_DanhGiaList", danhGiaViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhanHoiDanhGiaByMaDG(int maDG, int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var phanHoiList = await _phanHoiDanhGiaRepository.GetByMaDGPagedAsync(maDG, pageNumber, pageSize);
                var phanHoiViewModels = await TaoPhanHoiDanhGiaViewModelsAsync(phanHoiList);

                var tongSoPhanHoi = await _context.PhanHoiDanhGia.CountAsync(ph => ph.MaDG == maDG);
                var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

                foreach (var viewModel in phanHoiViewModels)
                {
                    viewModel.TrangHienTai = pageNumber;
                    viewModel.TongSoTrang = tongSoTrang;
                    viewModel.KichThuocTrang = pageSize;
                }

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                    ViewData["KhachHang"] = khachHang;
                }

                return PartialView("_PhanHoiDanhGiaList", phanHoiViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phản hồi đánh giá với maDG={maDG}", maDG);
                return StatusCode(500, "Lỗi server khi tải danh sách phản hồi.");
            }
        }

        // Lấy danh sách bình luận theo MaSP
        [HttpGet]
        public async Task<IActionResult> GetPhanHoiByMaSP(int maSP, int pageNumber = 1, int pageSize = 5)
        {
            var phanHoiList = await _binhLuanRepository.GetByMaSPPagedAsync(maSP, pageNumber, pageSize);
            var soLuongTraLoi = await GetSoLuongTraLoiAsync(phanHoiList);

            // Gọi TaoPhanHoiViewModels để Tạo danh sách PhanHoiViewModel
            var phanHoiViewModels = await TaoPhanHoiViewModelsAsync(phanHoiList, soLuongTraLoi);
            // Tính tổng số trang
            var tongSoPhanHoi = await _context.BinhLuan.CountAsync(ph => ph.MaSP == maSP);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

            // Cập nhật thông tin phân trang
            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = pageNumber;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                ViewData["KhachHang"] = khachHang;
            }

            

            return PartialView("_PhanHoiList", phanHoiViewModels);
        }

       

      

        // Lấy danh sách trả lời theo MaPH
        [HttpGet]
        public async Task<IActionResult> GetTraLoiByMaPH(int maPH, int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var traLoiList = await _traLoiRepository.GetByMaPHPagedAsync(maPH, pageNumber, pageSize);
                var traLoiViewModels = await TaoTraLoiViewModelsAsync(traLoiList);
                // Tính tổng số trang
                var tongSoTraLoi = await _context.PhanHoiBinhLuan.CountAsync(tl => tl.MaBL == maPH);
                var tongSoTrang = (int)Math.Ceiling((double)tongSoTraLoi / pageSize);

                // Cập nhật thông tin phân trang
                foreach (var viewModel in traLoiViewModels)
                {
                    viewModel.TrangHienTai = pageNumber;
                    viewModel.TongSoTrang = tongSoTrang;
                    viewModel.KichThuocTrang = pageSize;
                }

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                    ViewData["KhachHang"] = khachHang;
                }

                return PartialView("_TraLoiList", traLoiViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách trả lời với maPH={maPH}", maPH);
                return StatusCode(500, "Lỗi server khi tải danh sách trả lời.");
            }
        }


        /*-------------------------------- THÊM PHẢN HỒI- TRẢ LỜI----------------------------------------------------------------------*/

        [HttpPost]
        public async Task<IActionResult> AddDanhGia(int maSP, string noiDungDG, int sao)
        {
            if (!User.Identity.IsAuthenticated)
            {
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={maSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var cthds = await _cthdRepository.GetByMaKHAndMaSPAsync(khachHang.MaKH, maSP);
            if (!cthds.Any())
            {
                return Json(new { success = false, message = "Bạn chưa mua sản phẩm này." });
            }

            // Tìm CTHD chưa được đánh giá
            CTHD cthdChuaDanhGia = null;
            foreach (var cthd in cthds)
            {
                if (!await _danhGiaRepository.ExistsAsync(cthd.SoHD, cthd.MaSP, cthd.MaKho))
                {
                    cthdChuaDanhGia = cthd;
                    break;
                }
            }

            if (cthdChuaDanhGia == null)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này cho tất cả các hóa đơn. Mua thêm để đánh giá!" });
            }

            var danhGia = new DanhGia
            {
                MaKho = cthdChuaDanhGia.MaKho,
                MaSP = maSP,
                SoHD = cthdChuaDanhGia.SoHD,
                NoiDung = string.IsNullOrWhiteSpace(noiDungDG) ? null : noiDungDG,
                Sao = sao,
                NgayDG = DateTime.Now,
                TTHienThi = true
            };

            await _danhGiaRepository.AddAsync(danhGia);

            const int pageSize = 5;
            var danhGiaList = await _danhGiaRepository.GetByMaSPPagedAsync(maSP, 1, pageSize);
            var soLuongPhanHoiDanhGia = await GetSoLuongPhanHoiDanhGiaAsync(danhGiaList);
            var danhGiaViewModels = await TaoDanhGiaViewModelsAsync(danhGiaList, soLuongPhanHoiDanhGia);

            var tongSoDanhGia = await _context.DanhGia.CountAsync(dg => dg.MaSP == maSP && dg.TTHienThi == true);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoDanhGia / pageSize);

            foreach (var viewModel in danhGiaViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_DanhGiaList", danhGiaViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhanHoiDanhGia(int maDG, string noiDungPHDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var danhGia = await _danhGiaRepository.GetByIdAsync(maDG);
                if (danhGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhGia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            if (string.IsNullOrWhiteSpace(noiDungPHDG))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phản hồi." });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var phanHoiDanhGia = new PhanHoiDanhGia
            {
                MaDG = maDG,
                MaKH = khachHang.MaKH,
                NoiDungPHDG = noiDungPHDG,
                NgayPHDG = DateTime.Now
            };

            await _phanHoiDanhGiaRepository.AddAsync(phanHoiDanhGia);

            const int pageSize = 5;
            var phanHoiList = await _phanHoiDanhGiaRepository.GetByMaDGPagedAsync(maDG, 1, pageSize);
            var phanHoiViewModels = await TaoPhanHoiDanhGiaViewModelsAsync(phanHoiList);

            var tongSoPhanHoi = await _context.PhanHoiDanhGia.CountAsync(ph => ph.MaDG == maDG);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_PhanHoiDanhGiaList", phanHoiViewModels);
        }

        // Thêm bình luận mới
        [HttpPost]
        public async Task<IActionResult> AddPhanHoi(int maSP, string noiDungPH)
        {
            if (!User.Identity.IsAuthenticated)
            {
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={maSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            if (noiDungPH == null || noiDungPH.Trim() == "")
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung bình luận." });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var phanHoi = new BinhLuan
            {
                MaKH = khachHang.MaKH,
                MaSP = maSP,
                NoiDungBL = noiDungPH,
                NgayBL = DateTime.Now,
   
            };

            await _binhLuanRepository.AddAsync(phanHoi);

           
                const int pageSize = 5;
                var phanHoiList = await _binhLuanRepository.GetByMaSPPagedAsync(maSP, 1, pageSize);
                var soLuongTraLoi = await GetSoLuongTraLoiAsync(phanHoiList);
                var phanHoiViewModels = await TaoPhanHoiViewModelsAsync(phanHoiList, soLuongTraLoi);

                var tongSoPhanHoi = await _context.BinhLuan.CountAsync(ph => ph.MaSP == maSP);
                var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

                foreach (var viewModel in phanHoiViewModels)
                {
                    viewModel.TrangHienTai = 1;
                    viewModel.TongSoTrang = tongSoTrang;
                    viewModel.KichThuocTrang = pageSize;
                }

                ViewData["KhachHang"] = khachHang;
                return PartialView("_PhanHoiList", phanHoiViewModels);
            
        }

        // Thêm trả lời mới
        [HttpPost]
        public async Task<IActionResult> AddTraLoi(int maPH, string noiDungTL)
        {
            // Lấy MaSP từ MaPH để tạo returnUrl
            var phanHoi = await _binhLuanRepository.GetByIdAsync(maPH);
            // Kiểm tra xem đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Đường dẫn đến trang thanh toán (nơi quay lại sau)
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={phanHoi.MaSP}";

                // Đường dẫn đến trang đăng nhập
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";

                // Nói rõ rằng sau khi đăng nhập thì quay lại trang thanh toán
                string duongDanDayDu = trangDangNhap + trangTiepTuc;

                // Chuyển người dùng đến trang đăng nhập
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            if (noiDungTL == null || noiDungTL.Trim() == "")
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung trả lời." });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var traLoi = new PhanHoiBinhLuan
            {
                MaBL = maPH,
                MaKH = khachHang.MaKH,
                NoiDungPHBL = noiDungTL,
                NgayPHBL = DateTime.Now
            };

            await _traLoiRepository.AddAsync(traLoi);
            const int pageSize = 5;
            
            var traLoiList = await _traLoiRepository.GetByMaPHPagedAsync(maPH, 1, pageSize); // Tải trang đầu tiên           
            var traLoiViewModels = await TaoTraLoiViewModelsAsync(traLoiList);

            // Cập nhật thông tin phân trang
            var tongSoTraLoi = await _context.PhanHoiBinhLuan.CountAsync(tl => tl.MaBL == maPH);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoTraLoi / pageSize);
            foreach (var viewModel in traLoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_TraLoiList", traLoiViewModels);
        }


        /*-------------------------------- XỬA - XOÁ PHẢN HỒI----------------------------------------------------------------------*/

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditDanhGia(int maDG, string noiDungDG, int sao)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var danhgia = await _danhGiaRepository.GetByIdAsync(maDG);
                if (danhgia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhgia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var danhGia = await _danhGiaRepository.GetByIdAsync(maDG);
            if (danhGia == null || danhGia.CTHD?.HoaDon?.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa đánh giá này." });
            }

            danhGia.NoiDung = string.IsNullOrWhiteSpace(noiDungDG) ? null : noiDungDG;
            danhGia.Sao = sao;
            danhGia.NgayDG = DateTime.Now;
            await _danhGiaRepository.UpdateAsync(danhGia);

            const int pageSize = 5;
            var danhGiaList = await _danhGiaRepository.GetByMaSPPagedAsync(danhGia.MaSP, 1, pageSize);
            var soLuongPhanHoiDanhGia = await GetSoLuongPhanHoiDanhGiaAsync(danhGiaList);
            var danhGiaViewModels = await TaoDanhGiaViewModelsAsync(danhGiaList, soLuongPhanHoiDanhGia);

            var tongSoDanhGia = await _context.DanhGia.CountAsync(dg => dg.MaSP == danhGia.MaSP && dg.TTHienThi == true);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoDanhGia / pageSize);

            foreach (var viewModel in danhGiaViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_DanhGiaList", danhGiaViewModels);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteDanhGia(int maDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var danhgia = await _danhGiaRepository.GetByIdAsync(maDG);
                if (danhgia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhgia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var danhGia = await _danhGiaRepository.GetByIdAsync(maDG);
            if (danhGia == null || danhGia.CTHD?.HoaDon?.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa đánh giá này." });
            }

            await _danhGiaRepository.DeleteAsync(maDG);

            const int pageSize = 5;
            var danhGiaList = await _danhGiaRepository.GetByMaSPPagedAsync(danhGia.MaSP, 1, pageSize);
            var soLuongPhanHoiDanhGia = await GetSoLuongPhanHoiDanhGiaAsync(danhGiaList);
            var danhGiaViewModels = await TaoDanhGiaViewModelsAsync(danhGiaList, soLuongPhanHoiDanhGia);

            var tongSoDanhGia = await _context.DanhGia.CountAsync(dg => dg.MaSP == danhGia.MaSP && dg.TTHienThi == true);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoDanhGia / pageSize);

            foreach (var viewModel in danhGiaViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_DanhGiaList", danhGiaViewModels);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditPhanHoiDanhGia(int maPHDG, string noiDungPHDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var phanhoi = await _phanHoiDanhGiaRepository.GetByIdAsync(maPHDG);
                if (phanhoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phản hồi." });
                }
                var danhGia = await _danhGiaRepository.GetByIdAsync(phanhoi.MaDG);
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhGia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            if (string.IsNullOrWhiteSpace(noiDungPHDG))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phản hồi." });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var phanHoi = await _phanHoiDanhGiaRepository.GetByIdAsync(maPHDG);
            if (phanHoi == null || phanHoi.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa phản hồi này." });
            }

            // Chuyển @TênKháchHàng thành @KHx
            string noiDungLuu = await ConvertTenKHToKHxAsync(noiDungPHDG);

            phanHoi.NoiDungPHDG = noiDungLuu;
            phanHoi.NgayPHDG = DateTime.Now;
            await _phanHoiDanhGiaRepository.UpdateAsync(phanHoi);

            const int pageSize = 5;
            var phanHoiList = await _phanHoiDanhGiaRepository.GetByMaDGPagedAsync(phanHoi.MaDG, 1, pageSize);
            var phanHoiViewModels = await TaoPhanHoiDanhGiaViewModelsAsync(phanHoiList);

            var tongSoPhanHoi = await _context.PhanHoiDanhGia.CountAsync(ph => ph.MaDG == phanHoi.MaDG);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_PhanHoiDanhGiaList", phanHoiViewModels);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePhanHoiDanhGia(int maPHDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var phanhoi = await _phanHoiDanhGiaRepository.GetByIdAsync(maPHDG);
                if (phanhoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phản hồi." });
                }
                var danhGia = await _danhGiaRepository.GetByIdAsync(phanhoi.MaDG);
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhGia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            var phanHoi = await _phanHoiDanhGiaRepository.GetByIdAsync(maPHDG);
            if (phanHoi == null || phanHoi.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa phản hồi này." });
            }

            await _phanHoiDanhGiaRepository.DeleteAsync(maPHDG);

            const int pageSize = 5;
            var phanHoiList = await _phanHoiDanhGiaRepository.GetByMaDGPagedAsync(phanHoi.MaDG, 1, pageSize);
            var phanHoiViewModels = await TaoPhanHoiDanhGiaViewModelsAsync(phanHoiList);

            var tongSoPhanHoi = await _context.PhanHoiDanhGia.CountAsync(ph => ph.MaDG == phanHoi.MaDG);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);

            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            ViewData["KhachHang"] = khachHang;
            return PartialView("_PhanHoiDanhGiaList", phanHoiViewModels);
        }
        /*-------------------------------------------------------------------*/
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditPhanHoi(int maPH, string noiDungPH)
        {
          

            // Kiểm tra nội dung
            if (noiDungPH == null || noiDungPH.Trim() == "")
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung bình luận." });
            }

            // Lấy thông tin người dùng
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            // Tìm bình luận
            var phanHoi = await _binhLuanRepository.GetByIdAsync(maPH);
            if (phanHoi == null || phanHoi.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa bình luận này." });
            }

            // Cập nhật nội dung và thời gian
            phanHoi.NoiDungBL = noiDungPH;
            phanHoi.NgayBL = DateTime.Now;
            await _binhLuanRepository.UpdateAsync(phanHoi);

            // Tải lại danh sách bình luận
            const int pageSize = 5;
            var phanHoiList = await _binhLuanRepository.GetByMaSPPagedAsync(phanHoi.MaSP, 1, pageSize);
            var soLuongTraLoi = await GetSoLuongTraLoiAsync(phanHoiList);
            var phanHoiViewModels = await TaoPhanHoiViewModelsAsync(phanHoiList, soLuongTraLoi);
            var tongSoPhanHoi = await _context.BinhLuan.CountAsync(ph => ph.MaSP == phanHoi.MaSP);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);
            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }
            // Truyền thông tin khách hàng
            ViewData["KhachHang"] = khachHang;

            return PartialView("_PhanHoiList", phanHoiViewModels);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePhanHoi(int maPH)
        {
          
            // Lấy thông tin người dùng
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            // Tìm bình luận
            var phanHoi = await _binhLuanRepository.GetByIdAsync(maPH);
            if (phanHoi == null || phanHoi.MaKH != khachHang.MaKH)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa bình luận này." });
            }

            // Xóa bình luận
            await _binhLuanRepository.DeleteAsync(maPH);

            // Tải lại danh sách bình luận
            const int pageSize = 5;
            var phanHoiList = await _binhLuanRepository.GetByMaSPPagedAsync(phanHoi.MaSP, 1, pageSize);
            var soLuongTraLoi = await GetSoLuongTraLoiAsync(phanHoiList);
            var phanHoiViewModels = await TaoPhanHoiViewModelsAsync(phanHoiList, soLuongTraLoi);
            var tongSoPhanHoi = await _context.BinhLuan.CountAsync(ph => ph.MaSP == phanHoi.MaSP);
            var tongSoTrang = (int)Math.Ceiling((double)tongSoPhanHoi / pageSize);
            foreach (var viewModel in phanHoiViewModels)
            {
                viewModel.TrangHienTai = 1;
                viewModel.TongSoTrang = tongSoTrang;
                viewModel.KichThuocTrang = pageSize;
            }

            // Truyền thông tin khách hàng
            ViewData["KhachHang"] = khachHang;

            return PartialView("_PhanHoiList", phanHoiViewModels);
        }

        /*-------------------------------- XỬA - XOÁ TRẢ LỜI----------------------------------------------------------------------*/

        // Thêm vào HomeController

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTraLoi(int maTL, string noiDungTL)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(noiDungTL))
                {
                    return Json(new { success = false, message = "Vui lòng nhập nội dung trả lời." });
                }

                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                var traLoi = await _traLoiRepository.GetByIdAsync(maTL);
                if (traLoi == null || traLoi.MaKH != khachHang.MaKH)
                {
                    return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa trả lời này." });
                }

                // Chuyển @TênKháchHàng thành @KHx
                string noiDungLuu = await ConvertTenKHToKHxAsync(noiDungTL);

                traLoi.NoiDungPHBL = noiDungLuu;
                traLoi.NgayPHBL = DateTime.Now;
                await _traLoiRepository.UpdateAsync(traLoi);

                const int pageSize = 5;
                var traLoiList = await _traLoiRepository.GetByMaPHPagedAsync(traLoi.MaBL, 1, pageSize);
                var traLoiViewModels = await TaoTraLoiViewModelsAsync(traLoiList);

                var tongSoTraLoi = await _context.PhanHoiBinhLuan.CountAsync(tl => tl.MaBL == traLoi.MaBL);
                var tongSoTrang = (int)Math.Ceiling((double)tongSoTraLoi / pageSize);
                foreach (var viewModel in traLoiViewModels)
                {
                    viewModel.TrangHienTai = 1;
                    viewModel.TongSoTrang = tongSoTrang;
                    viewModel.KichThuocTrang = pageSize;
                }
                ViewData["KhachHang"] = khachHang;
                return PartialView("_TraLoiList", traLoiViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chỉnh sửa trả lời với maTL={maTL}", maTL);
                return Json(new { success = false, message = "Lỗi server khi chỉnh sửa trả lời." });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteTraLoi(int maTL)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                var traLoi = await _traLoiRepository.GetByIdAsync(maTL);
                if (traLoi == null || traLoi.MaKH != khachHang.MaKH)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa trả lời này." });
                }

                await _traLoiRepository.DeleteAsync(maTL);

                const int pageSize = 5;
                var traLoiList = await _traLoiRepository.GetByMaPHPagedAsync(traLoi.MaBL, 1, pageSize);
                var traLoiViewModels = await TaoTraLoiViewModelsAsync(traLoiList);

                var tongSoTraLoi = await _context.PhanHoiBinhLuan.CountAsync(tl => tl.MaBL == traLoi.MaBL);
                var tongSoTrang = (int)Math.Ceiling((double)tongSoTraLoi / pageSize);
                foreach (var viewModel in traLoiViewModels)
                {
                    viewModel.TrangHienTai = 1;
                    viewModel.TongSoTrang = tongSoTrang;
                    viewModel.KichThuocTrang = pageSize;
                }
                ViewData["KhachHang"] = khachHang;
                return PartialView("_TraLoiList", traLoiViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa trả lời với maTL={maTL}", maTL);
                return Json(new { success = false, message = "Lỗi server khi xóa trả lời." });
            }
        }

        /*-----------------------------------------Like phản hồi và trả lời-------------------------------------------------------------*/

        [HttpPost]
        public async Task<IActionResult> LikeDanhGia(int maDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var danhGia = await _danhGiaRepository.GetByIdAsync(maDG);
                if (danhGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhGia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?ReturnUrl=";
                string duongDanDayDu = trangDangNhap + Uri.EscapeDataString(trangTiepTuc);
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            bool daThich = await _likeDanhGiaRepository.ExistsAsync(khachHang.MaKH, maDG);
            if (daThich)
            {
                await _likeDanhGiaRepository.DeleteAsync(khachHang.MaKH, maDG);
            }
            else
            {
                var likeDanhGia = new LikeDanhGia
                {
                    MaKH = khachHang.MaKH,
                    MaDG = maDG,
                    TGThich = DateTime.Now
                };
                await _likeDanhGiaRepository.AddAsync(likeDanhGia);
            }

            var soLuotThich = (await _likeDanhGiaRepository.GetByMaDGAsync(maDG)).Count();
            return Json(new { success = true, daThich = !daThich, soLuotThich });
        }

        [HttpPost]
        public async Task<IActionResult> LikePhanHoiDanhGia(int maPHDG)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var phanHoi = await _phanHoiDanhGiaRepository.GetByIdAsync(maPHDG);
                if (phanHoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phản hồi." });
                }
                var danhGia = await _danhGiaRepository.GetByIdAsync(phanHoi.MaDG);
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={danhGia.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?ReturnUrl=";
                string duongDanDayDu = trangDangNhap + Uri.EscapeDataString(trangTiepTuc);
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            bool daThich = await _likePhanHoiDanhGiaRepository.ExistsAsync(khachHang.MaKH, maPHDG);
            if (daThich)
            {
                await _likePhanHoiDanhGiaRepository.DeleteAsync(khachHang.MaKH, maPHDG);
            }
            else
            {
                var likePhanHoiDanhGia = new LikePhanHoiDanhGia
                {
                    MaKH = khachHang.MaKH,
                    MaPHDG = maPHDG,
                    TGThich = DateTime.Now
                };
                await _likePhanHoiDanhGiaRepository.AddAsync(likePhanHoiDanhGia);
            }

            var soLuotThich = (await _likePhanHoiDanhGiaRepository.GetByMaPHDGAsync(maPHDG)).Count();
            return Json(new { success = true, daThich = !daThich, soLuotThich });
        }

        [HttpPost]
        public async Task<IActionResult> LikePhanHoi(int maPH)
{
            if (!User.Identity.IsAuthenticated)
            {
                var phanHoi = await _binhLuanRepository.GetByIdAsync(maPH);
                if (phanHoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bình luận." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={phanHoi.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?ReturnUrl=";
                string duongDanDayDu = trangDangNhap + Uri.EscapeDataString(trangTiepTuc);
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            bool daThich = await _likePhanHoiRepository.ExistsAsync(khachHang.MaKH, maPH);
            if (daThich)
            {
                await _likePhanHoiRepository.DeleteAsync(khachHang.MaKH, maPH);
            }
            else
            {
                var likePhanHoi = new LikeBinhLuan
                {
                    MaKH = khachHang.MaKH,
                    MaBL = maPH,
                    TGThich = DateTime.Now
                };
                await _likePhanHoiRepository.AddAsync(likePhanHoi);
            }

            var soLuotThich = (await _likePhanHoiRepository.GetByMaPHAsync(maPH)).Count();
            return Json(new { success = true, daThich = !daThich, soLuotThich });
        }


        [HttpPost]
        public async Task<IActionResult> LikeTraLoi(int maTL)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var traLoi = await _traLoiRepository.GetByIdAsync(maTL);
                if (traLoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy câu trả lời." });
                }
                var phanHoi = await _binhLuanRepository.GetByIdAsync(traLoi.MaBL);
                if (phanHoi == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bình luận." });
                }
                string trangTiepTuc = $"/Home/ChiTietSanPham?id={phanHoi.MaSP}";
                string trangDangNhap = "/Identity/Account/Login?ReturnUrl=";
                string duongDanDayDu = trangDangNhap + Uri.EscapeDataString(trangTiepTuc);
                return Json(new { success = false, redirect = duongDanDayDu });
            }

            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            bool daThich = await _likeTraLoiRepository.ExistsAsync(khachHang.MaKH, maTL);
            if (daThich)
            {
                await _likeTraLoiRepository.DeleteAsync(khachHang.MaKH, maTL);
            }
            else
            {
                var likeTraLoi = new LikePhanHoiBinhLuan
                {
                    MaKH = khachHang.MaKH,
                    MaPHBL = maTL,
                    TGThich = DateTime.Now
                };
                await _likeTraLoiRepository.AddAsync(likeTraLoi);
            }

            var soLuotThich = (await _likeTraLoiRepository.GetByMaTLAsync(maTL)).Count();
            return Json(new { success = true, daThich = !daThich, soLuotThich });
        }

        /*------------------------------------------------------------------------------------------------------*/
        [HttpPost]
        public async Task<IActionResult> ChatWithGeMini(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Vui lòng nhập câu hỏi." });
            }

            var grokService = HttpContext.RequestServices.GetService<GeminiService>();
            var response = await grokService.GetGrokResponseAsync(message);
            return Json(new { success = true, response });
        }
        /*------------------------------------------------------------------------------------------------------*/
        private async Task<string> ConvertKHxToTenKHAsync(string noiDung)
        {
            if (string.IsNullOrEmpty(noiDung) || !noiDung.StartsWith("@KH"))
                return noiDung;

            var match = System.Text.RegularExpressions.Regex.Match(noiDung, @"^@KH(\d+)\s");
            if (!match.Success)
                return noiDung;

            int maKH = int.Parse(match.Groups[1].Value);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.MaKH == maKH);
            if (khachHang == null)
                return noiDung;

            string tenKH = khachHang.TenKH ?? "Unknown";
            // Bọc tag trong span với class "user-tag"
            return $"<span class='user-tag'>@{tenKH}</span> {noiDung.Substring(match.Length).Trim()}";
        }

        private async Task<string> ConvertTenKHToKHxAsync(string noiDung)
        {
            if (string.IsNullOrEmpty(noiDung) || !noiDung.StartsWith("@"))
                return noiDung;

            // Trích xuất tên khách hàng sau @
            var match = System.Text.RegularExpressions.Regex.Match(noiDung, @"^@([^\s]+(?:\s+[^\s]+)*)\s");
            if (!match.Success)
                return noiDung;

            string tenKH = match.Groups[1].Value;
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.TenKH == tenKH);
            if (khachHang == null)
                return noiDung;

            // Thay @TenKH bằng @KHx
            return $"@KH{khachHang.MaKH} {noiDung.Substring(match.Length).Trim()}";
        }

        [HttpGet]
        public async Task<IActionResult> GetNoiDungForEdit(string type, int id)
        {
            string noiDung = string.Empty;
            if (type == "PhanHoiDanhGia")
            {
                var phanHoi = await _phanHoiDanhGiaRepository.GetByIdAsync(id);
                if (phanHoi == null)
                    return Json(new { success = false, message = "Không tìm thấy phản hồi." });
                noiDung = await ConvertKHxToTenKHAsync(phanHoi.NoiDungPHDG);
            }
            else if (type == "TraLoi")
            {
                var traLoi = await _traLoiRepository.GetByIdAsync(id);
                if (traLoi == null)
                    return Json(new { success = false, message = "Không tìm thấy trả lời." });
                noiDung = await ConvertKHxToTenKHAsync(traLoi.NoiDungPHBL);
            }
            else
            {
                return Json(new { success = false, message = "Loại nội dung không hợp lệ." });
            }

            return Json(new { success = true, noiDung });

        }

        public IActionResult Privacy()
        {
            LaySoLuongGioHang().GetAwaiter().GetResult(); // Gọi đồng bộ vì Privacy không async
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            LaySoLuongGioHang().GetAwaiter().GetResult(); // Gọi đồng bộ
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }



   

}