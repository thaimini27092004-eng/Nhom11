
/*SanPhamControllter.cs*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Models;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.I;
using System.Linq;
using NuGet.Packaging.Signing;
using WebsiteBanHang.Repositories.I.QLKhoHang;
using WebsiteBanHang.Models.QLTonKho;
using WebsiteBanHang.Repositories.I.QuanLyHoaDon;
using WebsiteBanHang.Repositories.EF;

namespace WebsiteBanHang.Areas.Admin.Controllers
    {
    [Area("Admin")]
    public class SanPhamController : Controller
        {
            private readonly ISanPhamRepository _sanphamRepository;
            private readonly IDanhMucRepository _danhmucRepository;
            private readonly IDSAnhRepository _dsAnhRepository;
            private readonly ICTHDRepository _cthdRepository;
            private readonly ITonKhoRepository _tonKhoRepository;
            private readonly IKhoRepository _khoRepository;
            private readonly IQuanLyHoaDonRepository _hoaDonRepository;
            private readonly ApplicationDbContext _context; // Thêm để truy cập trực tiếp vào DbContext


        public SanPhamController(ISanPhamRepository sanphamRepository,
                IDanhMucRepository danhmucRepository,
                IDSAnhRepository dsAnhRepository,
                ICTHDRepository cthdRepository,
                ITonKhoRepository tonKhoRepository,
                IKhoRepository khoRepository,
                IQuanLyHoaDonRepository hoaDonRepository,
                ApplicationDbContext context)
            {
                _hoaDonRepository = hoaDonRepository;
                _sanphamRepository = sanphamRepository;
                _danhmucRepository = danhmucRepository;
                _dsAnhRepository = dsAnhRepository;
                _cthdRepository = cthdRepository;
                _tonKhoRepository = tonKhoRepository;
                _khoRepository = khoRepository;
            _context = context;
        }

        private async Task ganDanhMucVaoViewBag()
        {
            var danhmucs = (await _danhmucRepository.GetAllAsync()).Where(c => c.TTHienThi).ToList();
            ViewBag.danhmucs = danhmucs;
        }
        private async Task ganKhoHangVaoViewBag()
        {
            var khos = (await _khoRepository.GetAllAsync()).Where(k => k.TTHienThi).ToList();
            ViewBag.khos = khos;
        }

        /*------------------------------------------------------------------------------------------------------------------*/
        //  Hiển thị danh sách sản phẩm và xử lý tìm kiếm
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Index(string tuKhoaTimKiem = "", string chonTrangThai = "batHienThi", string chonDanhMuc = "", string sapXepGia = "macDinh", int? chonKho = null)
        {
            // Lấy tất cả sản phẩm từ database và chuyển thành List để dùng chỉ số
            var sanphams = (await _sanphamRepository.GetAllAsync()).ToList();
            
            // Lấy danh mục categories hiển thị
            await ganDanhMucVaoViewBag();
            await ganKhoHangVaoViewBag();

            // Lưu từ khóa tìm kiếm để hiển thị lại trên giao diện
            ViewData["tuKhoaTimKiem"] = tuKhoaTimKiem;
            ViewData["chonTrangThai"] = chonTrangThai;
            ViewData["chonDanhMuc"] = chonDanhMuc;
            ViewData["sapXepGia"] = sapXepGia;
            ViewData["chonKho"] = chonKho;

            // Lọc theo trạng thái
            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    sanphams = sanphams.Where(p => !p.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    sanphams = sanphams.Where(p => p.TTHienThi).ToList();
                    break;
            }

            // Lọc theo danh mục nếu có giá trị chonDanhMuc
            int maDM = 0; // Giá trị mặc định
            bool coTheLoc = false;

            if (chonDanhMuc != "" && chonDanhMuc != null) // Kiểm tra không rỗng và không null
            {
                // Kiểm tra xem chuỗi có phải toàn số không
                bool toanSo = true;
                for (int i = 0; i < chonDanhMuc.Length; i++)
                {
                    if (!char.IsDigit(chonDanhMuc[i]))
                    {
                        toanSo = false;
                        break;
                    }
                }

                if (toanSo) // Nếu toàn số, chuyển thành int
                {
                    maDM = int.Parse(chonDanhMuc);
                    coTheLoc = true;
                }
            }


            if (coTheLoc) // Nếu chuyển được, lọc sản phẩm
            {
                sanphams = sanphams.Where(p => p.MaDM == maDM).ToList();
            }

            // Lọc theo kho
            if (chonKho.HasValue)
            {
                sanphams = (await _sanphamRepository.GetByMaKhoAsync(chonKho.Value)).ToList();
            }

            // Lấy danh sách tồn kho và lưu vào ViewBag
            var tonKhoDict = new Dictionary<int, List<(int MaKho, string TenKho, int SLTon)>>();
            foreach (var sanpham in sanphams)
            {
                var tonKhos = await _tonKhoRepository.GetByMaSPAsync(sanpham.MaSP);
                var tonKhoList = tonKhos.Select(tk => (tk.MaKho, tk.Kho?.TenKho ?? "Không xác định", tk.SLTon)).ToList();
                tonKhoDict[sanpham.MaSP] = tonKhoList;
            }
            ViewBag.TonKhoDict = tonKhoDict;


            // Nếu không có từ khóa, trả về toàn bộ danh sách sản phẩm
            if (tuKhoaTimKiem == "")
            //if (string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                return View(sanphams);
            }

            // Kiểm tra nếu từ khóa chứa dấu phẩy (tìm kiếm theo mã sản phẩm)
            if (tuKhoaTimKiem.Contains(","))
            {
                var idArray = tuKhoaTimKiem.Split(',');
                var danhSachIdHopLe = new List<int>();

                // Duyệt qua từng phần tử trong mảng idArray
                for (int i = 0; i < idArray.Length; i++)
                {
                    var id = idArray[i].Trim();
                    if (int.TryParse(id, out int idSanPham))
                    {
                        danhSachIdHopLe.Add(idSanPham);
                    }
                }

                // Tạo danh sách kết quả tìm kiếm
                var ketQua_TimKiem = new List<SanPham>();
                for (int i = 0; i < sanphams.Count; i++)
                {
                    for (int j = 0; j < danhSachIdHopLe.Count; j++)
                    {
                        if (sanphams[i].MaSP == danhSachIdHopLe[j])
                        {
                            ketQua_TimKiem.Add(sanphams[i]);
                            break; // Thoát vòng lặp trong nếu đã tìm thấy
                        }
                    }
                }

                // Kiểm tra kết quả
                if (ketQua_TimKiem.Count > 0)
                {
                    return View(ketQua_TimKiem);
                }
                else
                {
                    ViewBag.ketQuaTimKiem = "Không tìm thấy sản phẩm với mã đã nhập.";
                    var goiY = new List<SanPham>();
                    for (int i = 0; i < 3 && i < sanphams.Count; i++)
                    {
                        goiY.Add(sanphams[i]);
                    }
                    return View(goiY);
                }
            }

            // Chia từ khóa thành mảng các từ, chuyển thành chữ thường
            var tuKhoaArray = tuKhoaTimKiem.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // Tạo danh sách để lưu sản phẩm khớp
            var ketQuaTimKiem = new List<SanPham>();
            // Tạo danh sách để lưu điểm số tương ứng
            var diemSo = new List<int>();


            // Duyệt qua từng sản phẩm trong danh sách
            for (int i = 0; i < sanphams.Count; i++)
            {
                // Tính điểm khớp cho sản phẩm hiện tại
                int diem = TinhSoTuKhop(sanphams[i], tuKhoaArray);
                // Nếu điểm lớn hơn 0, tức là có từ khóa khớp
                if (diem > 0)
                {
                    // Thêm sản phẩm vào danh sách kết quả
                    ketQuaTimKiem.Add(sanphams[i]);
                    // Thêm điểm vào danh sách điểm số
                    diemSo.Add(diem);
                }
            }

            // Sắp xếp kết quả theo điểm giảm dần (bubble sort)
            for (int i = 0; i < ketQuaTimKiem.Count - 1; i++)
            {
                // So sánh từng cặp phần tử
                for (int j = i + 1; j < ketQuaTimKiem.Count; j++)
                {
                    // Nếu điểm trước nhỏ hơn điểm sau
                    if (diemSo[i] < diemSo[j])
                    {
                        // Hoán đổi sản phẩm
                        var tempProduct = ketQuaTimKiem[i];
                        ketQuaTimKiem[i] = ketQuaTimKiem[j];
                        ketQuaTimKiem[j] = tempProduct;
                        // Hoán đổi điểm tương ứng
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            // Nếu có kết quả tìm kiếm
            if (ketQuaTimKiem.Count > 0)
            {
                // Trả về danh sách sản phẩm đã sắp xếp
                return View(ketQuaTimKiem);
            }
            else
            {
                // Nếu không tìm thấy, hiển thị thông báo
                ViewBag.KetQuaTimKiem = "Không có sản phẩm liên quan.";
                // Tạo danh sách gợi ý 3 sản phẩm đầu tiên
                var goiY = new List<SanPham>();
                // Lấy tối đa 3 sản phẩm
                for (int i = 0; i < 3 && i < sanphams.Count; i++)
                {
                    goiY.Add(sanphams[i]);
                }
                // Trả về danh sách gợi ý
                return View(goiY);
            }


           
        }

        // Action SearchSuggestions: Trả về gợi ý thu nhỏ khi gõ ký tự
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, string chonTrangThai = "", int? chonKho = null)
        {
            // Lấy tất cả sản phẩm từ database và chuyển thành List để dùng chỉ số
            var sanphams = (await _sanphamRepository.GetAllAsync()).ToList();


            switch (chonTrangThai)
            {
                case "tatCa":
                    break;
                case "tatHienThi":
                    sanphams = sanphams.Where(p => !p.TTHienThi).ToList();
                    break;
                case "batHienThi":
                default:
                    sanphams = sanphams.Where(p => p.TTHienThi).ToList();
                    break;
            }

            // Lọc theo kho
            if (chonKho.HasValue)
            {
                sanphams = (await _sanphamRepository.GetByMaKhoAsync(chonKho.Value)).ToList();
            }

            // Chuyển từ khóa thành chữ thường
            string tuKhoa = term.ToLower();
            // Tạo danh sách để lưu gợi ý
            var goiY = new List<object>();
            // Tạo danh sách để lưu điểm số
            var diemSo = new List<int>();

            // Kiểm tra nếu từ khóa chứa dấu phẩy (tìm kiếm theo mã sản phẩm)
            if (tuKhoa.Contains(","))
            {
                var idArray = tuKhoa.Split(',');
                var danhSachIdHopLe = new List<int>();

                // Duyệt qua từng phần tử trong mảng idArray
                for (int i = 0; i < idArray.Length; i++)
                {
                    var id = idArray[i].Trim();
                    if (int.TryParse(id, out int idSanPham))
                    {
                        danhSachIdHopLe.Add(idSanPham);
                    }
                }

                // Tạo danh sách gợi ý
                int dem = 0;
                for (int i = 0; i < sanphams.Count && dem < 3; i++)
                {
                    for (int j = 0; j < danhSachIdHopLe.Count; j++)
                    {
                        if (sanphams[i].MaSP == danhSachIdHopLe[j])
                        {
                            goiY.Add(new
                            {
                                maSP = sanphams[i].MaSP,
                                tenSP = sanphams[i].TenSP,
                                urlAnh = sanphams[i].UrlAnh,
                                gia = sanphams[i].Gia
                            });
                            dem++;
                            break; // Thoát vòng lặp trong nếu đã thêm
                        }
                    }
                }

                // Trả về kết quả dưới dạng Json
                return Json(goiY);
            }

            // Duyệt qua từng sản phẩm
            for (int i = 0; i < sanphams.Count; i++)
            {
                // Tính điểm khớp cho sản phẩm hiện tại
                int diem = TinhSoKyTuKhop(sanphams[i], tuKhoa);
                // Nếu điểm lớn hơn 0, tức là có từ khóa khớp
                if (diem > 0)
                {
                    // Thêm thông tin sản phẩm vào danh sách gợi ý
                    goiY.Add(new
                    {
                        maSP = sanphams[i].MaSP,
                        tenSP = sanphams[i].TenSP,
                        urlAnh = sanphams[i].UrlAnh,
                        gia = sanphams[i].Gia
                    });
                    // Thêm điểm vào danh sách điểm số
                    diemSo.Add(diem);
                }
            }

            // Sắp xếp gợi ý theo điểm giảm dần (bubble sort)
            for (int i = 0; i < goiY.Count - 1; i++)
            {
                for (int j = i + 1; j < goiY.Count; j++)
                {
                    // Nếu điểm trước nhỏ hơn điểm sau
                    if (diemSo[i] < diemSo[j])
                    {
                        // Hoán đổi gợi ý
                        var tempGoiY = goiY[i];
                        goiY[i] = goiY[j];
                        goiY[j] = tempGoiY;
                        // Hoán đổi điểm tương ứng
                        var tempDiem = diemSo[i];
                        diemSo[i] = diemSo[j];
                        diemSo[j] = tempDiem;
                    }
                }
            }

            // Tạo danh sách kết quả với tối đa 3 gợi ý
            var ketQua = new List<object>();

            // Lấy tối đa 3 phần tử từ danh sách đã sắp xếp
            for (int i = 0; i < 3 && i < goiY.Count; i++)
            {
                ketQua.Add(goiY[i]);
            }

            // Trả về danh sách gợi ý dưới dạng Json
            return Json(ketQua);
        }

        // Hàm tính số từ khớp (dùng cho tìm kiếm chính khi nhấn Enter)
        private int TinhSoTuKhop(SanPham sanPham, string[] tuKhoaArray)
        {
            // Khởi tạo biến đếm số từ khớp
            int soTuKhop = 0;
            // Thêm khoảng trắng vào tên để tránh lỗi khi so khớp từ
            var tenSanPham = (" " + (sanPham.TenSP?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // Thêm khoảng trắng vào mô tả để tránh lỗi khi so khớp từ
            var moTaSanPham = (" " + (sanPham.MoTa?.ToLower() ?? "") + " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Duyệt qua từng từ khóa trong mảng từ khóa
            foreach (var tuKhoa in tuKhoaArray)
            {
                // Nếu từ khóa nằm trong tên sản phẩm
                if (tenSanPham.Contains(tuKhoa))
                    soTuKhop += 2; // Tên khớp, cộng 2 điểm
                // Nếu từ khóa nằm trong mô tả sản phẩm
                else if (moTaSanPham.Contains(tuKhoa))
                    soTuKhop += 1; // Mô tả khớp, cộng 1 điểm
            }
            // Trả về tổng điểm
            return soTuKhop;
        }

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

        //--------------------------------------------------------------------------------------------------------------------------------------
        private async Task ganKhoVaoViewBag()
        {
            var khos = await _khoRepository.GetDisplayedAsync();
            ViewBag.khos = new SelectList(khos, "MaKho", "TenKho");
        }

        private async Task ganDanhMucChoViewBag()
        {
            var danhmucs = await _danhmucRepository.GetDisplayedAsync();
            ViewBag.danhmucs = new SelectList(danhmucs, "MaDM", "TenDM");
        }

        // Hiển thị form thêm sản phẩm mới
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add()
        {
            await ganDanhMucChoViewBag();
            await ganKhoVaoViewBag();
            return View();
        }
        

        //Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(SanPham sanpham, IFormFile urlAnh, List<IFormFile> additionalImages, int? maKho, int? slTon)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh đại diện
                if (urlAnh != null && urlAnh.Length > 0)
                {
                    sanpham.UrlAnh = await SaveImage(urlAnh, "anhchinhsanpham");
                }

                // Khởi tạo danh sách Images nếu chưa có
                sanpham.DSAnh = new List<DSAnh>();

                // Thêm sản phẩm trước để có Id
                await _sanphamRepository.AddAsync(sanpham);

               

                // Xử lý danh sách ảnh bổ sung
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    foreach (var dsAnh in additionalImages)
                    {
                        if (dsAnh != null && dsAnh.Length > 0)
                        {
                            var imagePath = await SaveImage(dsAnh, "dsanhsanpham");
                            var dsAnhs = new DSAnh
                            {
                                UrlAnh = imagePath,
                                MaSP = sanpham.MaSP
                            };
                            await _dsAnhRepository.AddAsync(dsAnhs);
                            sanpham.DSAnh.Add(dsAnhs);
                        }
                    }
                }

                // Xử lý thông tin tồn kho
                if (maKho.HasValue && slTon.HasValue && slTon.Value > 0)
                {
                    var tonKho = new TonKho
                    {
                        MaKho = maKho.Value,
                        MaSP = sanpham.MaSP,
                        SLTon = slTon.Value
                    };
                    await _context.TonKho.AddAsync(tonKho);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            await ganDanhMucChoViewBag();
            await ganKhoVaoViewBag();
            return View(sanpham);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveImage(IFormFile image, string folderName)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine("wwwroot/images", folderName, fileName); // Sử dụng folderName để chỉ định thư mục
            var relativePath = $"/images/{folderName}/{fileName}"; // Đường dẫn tương đối

            // Tạo thư mục nếu chưa tồn tại
            var directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return relativePath; // Trả về đường dẫn tương đối
        }

        //Nhớ tạo folder images trong wwwroot


        // Hiển thị thông tin chi tiết sản phẩm
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Display(int id)
        {
            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            return View(sanpham);
        }


 //--------------------------------------------------------------------------------------------------------------------------------------

        // Hiển thị form cập nhật sản phẩm
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Update(int id)
        {
            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            var danhmucs = await _danhmucRepository.GetAllAsync();
            ViewBag.danhmucs = new SelectList(danhmucs, "MaDM", "TenDM",
           sanpham.MaDM);
            // Lấy danh sách kho liên kết với sản phẩm từ TonKho
            var tonKhos = await _tonKhoRepository.GetByMaSPAsync(id);
            ViewBag.tonKhos = tonKhos;
            return View(sanpham);
        }


        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, SanPham sanpham, IFormFile urlAnh, List<IFormFile> additionalImages, List<int> DSAnhToDelete, int? maKho, int? slTonKho)
        {
            ModelState.Remove("UrlAnh"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != sanpham.MaSP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existinSanpham = await _sanphamRepository.GetByIdAsync(id);
                if (existinSanpham == null)
                {
                    return NotFound();
                }

                // Xử lý ảnh đại diện (ImageUrl) - Chỉ cập nhật nếu có file mới
                if (urlAnh != null && urlAnh.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existinSanpham.UrlAnh))
                    {
                        var oldPath = Path.Combine("wwwroot", existinSanpham.UrlAnh.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    existinSanpham.UrlAnh = await SaveImage(urlAnh, "anhchinhsanpham");
                }
                else
                {
                    existinSanpham.UrlAnh = existinSanpham.UrlAnh; // Giữ nguyên
                }

                // Xử lý xóa ảnh từ ImagesToDelete
                if (DSAnhToDelete != null && DSAnhToDelete.Any())
                {
                    foreach (var maAnh in DSAnhToDelete)
                    {
                        var dsAnhToDelete = existinSanpham.DSAnh?.FirstOrDefault(img => img.MaAnh == maAnh);
                        if (dsAnhToDelete != null)
                        {
                            var imagePath = Path.Combine("wwwroot", dsAnhToDelete.UrlAnh.TrimStart('/'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                            await _dsAnhRepository.DeleteAsync(maAnh);
                        }
                    }
                    // Cập nhật lại danh sách Images sau khi xóa
                    existinSanpham.DSAnh = existinSanpham.DSAnh?.Where(img => !DSAnhToDelete.Contains(img.MaAnh)).ToList() ?? new List<DSAnh>();
                }

                // Xử lý thêm mới ảnh từ additionalImages
                if (additionalImages != null && additionalImages.Any())
                {
                    foreach (var dsAnh in additionalImages)
                    {
                        if (dsAnh != null && dsAnh.Length > 0)
                        {
                            var imagePath = await SaveImage(dsAnh, "dsanhsanpham");
                            if (imagePath != null)
                            {
                                var dsAnhs = new DSAnh
                                {
                                    UrlAnh = imagePath,
                                    MaSP = existinSanpham.MaSP
                                };
                                await _dsAnhRepository.AddAsync(dsAnhs);
                                existinSanpham.DSAnh = existinSanpham.DSAnh ?? new List<DSAnh>();
                                existinSanpham.DSAnh.Add(dsAnhs);
                            }
                        }
                    }
                }

                // Cập nhật thông tin sản phẩm
                existinSanpham.TenSP = sanpham.TenSP;
                existinSanpham.Gia = sanpham.Gia;
                existinSanpham.MoTa = sanpham.MoTa;
                existinSanpham.MaDM = sanpham.MaDM;
                existinSanpham.TonKho = sanpham.TonKho;
                // Xử lý cập nhật số lượng tồn kho
                if (maKho.HasValue && slTonKho.HasValue)
                {
                    var tonKho = await _tonKhoRepository.GetByIdAsync(maKho.Value, id);
                    if (tonKho != null)
                    {
                        tonKho.SLTon = slTonKho.Value;
                        await _tonKhoRepository.UpdateAsync(tonKho);
                    }
                }
                await _sanphamRepository.UpdateAsync(existinSanpham);

                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, truyền lại dữ liệu
            var danhmucs = await _danhmucRepository.GetAllAsync();
            ViewBag.danhmucs = new SelectList(danhmucs, "MaDM", "TenDM");
            var tonKhos = await _tonKhoRepository.GetByMaSPAsync(id);
            ViewBag.tonKhos = tonKhos;
            return View(sanpham);

        }


        //--------------------------------------------------------------------------------------------------------------------------------------
        //ẩn danh sách sản phẩm
        private async Task HideSanPhamsAsync(List<int> sanphamMaSP)
        {
            foreach (var maSP in sanphamMaSP)
            {
                var sanpham = await _sanphamRepository.GetByIdAsync(maSP);
                if (sanpham != null)
                {
                    sanpham.TTHienThi = false;
                    await _sanphamRepository.UpdateAsync(sanpham);
                }
            }
        }

        // Hiển thị form xác nhận xóa sản phẩm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            return View(sanpham);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sanphamRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSanPhamInCTHD(List<int> selectedSanPhamMaSP)
        {
            Console.WriteLine("Selected IDs received: " + string.Join(", ", selectedSanPhamMaSP));
            var allCTHDs = await _cthdRepository.GetAllAsync();
            var result = new List<object>();

            for (int i = 0; i < selectedSanPhamMaSP.Count; i++)
            {
                int maSP = selectedSanPhamMaSP[i];
                var sanpham = await _sanphamRepository.GetByIdAsync(maSP);
                bool hasOrder = false;

                for (int j = 0; j < allCTHDs.Count(); j++)
                {
                    if (allCTHDs.ElementAt(j).MaSP == maSP)
                    {
                        hasOrder = true;
                        break;
                    }
                }

                result.Add(new { maSP = sanpham.MaSP, tenSP = sanpham.TenSP, hasOrder = hasOrder });
            }

            Console.WriteLine("Result count: " + result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var item = (dynamic)result[i];
                Console.WriteLine($"Result[{i}]: maSP: {item.maSP}, tenSP: {item.tenSP}, HasOrder: {item.hasOrder}");
            }
            return Json(result);
        }


        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhieuSanPham(List<int> selectedSanPhamMaSP, bool isDelete = false)
        {
            if (selectedSanPhamMaSP == null || selectedSanPhamMaSP.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để xử lý.";
                return RedirectToAction("Index");
            }

            var allCTHDs = await _cthdRepository.GetAllAsync();
            var sanphamToDelete = new List<int>();
            var sanphamToHide = new List<int>();

            // Chia sản phẩm thành 2 danh sách
            for (int i = 0; i < selectedSanPhamMaSP.Count; i++)
            {
                int sanphamMaSP = selectedSanPhamMaSP[i];
                bool hasOrder = false;

                for (int j = 0; j < allCTHDs.Count(); j++)
                {
                    if (allCTHDs.ElementAt(j).MaSP == sanphamMaSP)
                    {
                        hasOrder = true;
                        break;
                    }
                }

                if (hasOrder)
                {
                    sanphamToHide.Add(sanphamMaSP);
                }
                else
                {
                    sanphamToDelete.Add(sanphamMaSP);
                }
            }

            // Xử lý theo nút được nhấn
            if (isDelete) // Nút "Xóa"
            {
                // Xóa các sản phẩm không có hóa đơn
                for (int i = 0; i < sanphamToDelete.Count; i++)
                {
                    await _sanphamRepository.DeleteAsync(sanphamToDelete[i]);
                }

                // Ẩn các sản phẩm có hóa đơn
                for (int i = 0; i < sanphamToHide.Count; i++)
                {
                    var sanpham = await _sanphamRepository.GetByIdAsync(sanphamToHide[i]);
                    if (sanpham != null)
                    {
                        sanpham.TTHienThi = false;
                        await _sanphamRepository.UpdateAsync(sanpham);
                    }
                }

                if (sanphamToDelete.Count > 0)
                {
                    TempData["Message"] = $"Đã xóa {sanphamToDelete.Count} sản phẩm không có hóa đơn.";
                }
                if (sanphamToHide.Count > 0)
                {
                    TempData["Message"] = (TempData["Message"] ?? "") + $" Đã ẩn {sanphamToHide.Count} sản phẩm có hóa đơn.";
                }
            }
            else // Nút "Ẩn"
            {
                // Chỉ ẩn tất cả sản phẩm được chọn
                for (int i = 0; i < selectedSanPhamMaSP.Count; i++)
                {
                    var sanpham = await _sanphamRepository.GetByIdAsync(selectedSanPhamMaSP[i]);
                    if (sanpham != null)
                    {
                        sanpham.TTHienThi = false;
                        await _sanphamRepository.UpdateAsync(sanpham);
                    }
                }
                TempData["Message"] = $"Đã ẩn {selectedSanPhamMaSP.Count} sản phẩm.";
            }

            return RedirectToAction("Index");
        }


        // Thay đổi trạng thái hiển thị
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool status, string chonTrangThai)
        {
            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            sanpham.TTHienThi = status;
            await _sanphamRepository.UpdateAsync(sanpham);

            return Json(new { success = true});
        }

        //--------------------------------------------------------------------------------------------------------------------
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> LayDanhMucHienThi()
        {
            // Lấy tất cả danh mục từ database
            var tatCaDanhMuc = (await _danhmucRepository.GetAllAsync()).ToList();
            var danhSachDanhMuc = new List<object>();

            // Duyệt qua từng danh mục để lọc những danh mục hiển thị
            for (int i = 0; i < tatCaDanhMuc.Count; i++)
            {
                if (tatCaDanhMuc[i].TTHienThi) // Chỉ lấy danh mục có TrangThaiHienThi = true
                {
                    danhSachDanhMuc.Add(new
                    {
                        maDM = tatCaDanhMuc[i].MaDM,
                        tenDM = tatCaDanhMuc[i].TenDM
                    });
                }
            }

            return Json(danhSachDanhMuc);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatDanhMuc(string selectedSanPhamMaSP, int newDanhMucMaDM)
        {
            if (string.IsNullOrEmpty(selectedSanPhamMaSP))//Kiểm tra dữ liệu đầu vào có rỗng không
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một sản phẩm để cập nhật." });
            }

            // Chuyển chuỗi ID thành danh sách số nguyên
            var danhSachMaSPSanPham = new List<int>();
            var mangId = selectedSanPhamMaSP.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int maSP))
                {
                    danhSachMaSPSanPham.Add(maSP);
                }
            }

            if (danhSachMaSPSanPham.Count == 0) //kiểm tra xem danh sách này có chứa bất kỳ ID nào hợp lệ hay không.
            {
                return Json(new { success = false, message = "Danh sách sản phẩm không hợp lệ." });
            }

            // Kiểm tra danh mục mới có tồn tại và đang hiển thị không (tránh danh mục bị xoá trong quá trình đổi)
            var danhMucMoi = await _danhmucRepository.GetByIdAsync(newDanhMucMaDM);
            if (danhMucMoi == null || !danhMucMoi.TTHienThi)
            {
                return Json(new { success = false, message = "Danh mục được chọn không hợp lệ hoặc không hiển thị." });
            }

            // Cập nhật danh mục cho từng sản phẩm
            var updateSanPham = new List<object>();
            for (int i = 0; i < danhSachMaSPSanPham.Count; i++)
            {
                var sanPham = await _sanphamRepository.GetByIdAsync(danhSachMaSPSanPham[i]);
                if (sanPham != null)
                {
                    var danhMucCu = await _danhmucRepository.GetByIdAsync(sanPham.MaDM);
                    sanPham.MaDM = newDanhMucMaDM;
                    await _sanphamRepository.UpdateAsync(sanPham);
                    updateSanPham.Add(new
                    {
                        maSP = sanPham.MaSP,
                        tenSP = sanPham.TenSP,
                        danhMucCu = danhMucCu?.TenDM ?? "Không có",
                        danhMucMoi = danhMucMoi.TenDM
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật danh mục cho {danhSachMaSPSanPham.Count} sản phẩm.",
                updatedSanPham = updateSanPham
            });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(string selectedSanPhamMaSP, bool newTrangThai)
        {
            if (string.IsNullOrEmpty(selectedSanPhamMaSP))
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một sản phẩm để cập nhật." });
            }

            var danhSachMaSP = new List<int>();
            var mangId = selectedSanPhamMaSP.Split(',');
            for (int i = 0; i < mangId.Length; i++)
            {
                if (int.TryParse(mangId[i].Trim(), out int maSP))
                {
                    danhSachMaSP.Add(maSP);
                }
            }

            if (danhSachMaSP.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách sản phẩm không hợp lệ." });
            }

            var updateSanPham = new List<object>();
            for (int i = 0; i < danhSachMaSP.Count; i++)
            {
                var sanPham = await _sanphamRepository.GetByIdAsync(danhSachMaSP[i]);
                if (sanPham != null)
                {
                    var trangThaiCu = sanPham.TTHienThi;
                    sanPham.TTHienThi = newTrangThai;
                    await _sanphamRepository.UpdateAsync(sanPham);
                    updateSanPham.Add(new
                    {
                        maSP = sanPham.MaSP,
                        tenSP = sanPham.TenSP,
                        trangThaiCu = trangThaiCu ? "Bật" : "Tắt",
                        trangThaiMoi = newTrangThai ? "Bật" : "Tắt"
                    });
                }
            }

            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái cho {danhSachMaSP.Count} sản phẩm.",
                updatedSanPham = updateSanPham
            });
        }

    }
}
