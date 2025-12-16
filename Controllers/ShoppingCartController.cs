using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models.GioHang;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;
using Microsoft.AspNetCore.Identity;
using WebsiteBanHang.Models.NguoiDung;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebsiteBanHang.Models.VnPay;
using WebsiteBanHang.Services.VnPay;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using WebsiteBanHang.Models.VaiTro;
using WebsiteBanHang.Repositories.EF;

namespace WebsiteBanHang.Controllers
{
    public class ShoppingCartController : Controller
    {
        // Các repository và dịch vụ để làm việc với giỏ hàng, sản phẩm, người dùng, và thanh toán
        private readonly ISanPhamRepository _productRepository;
        private readonly IGioHangRepository _luuGioHangRepository;
        private readonly ICTGioHangRepository _ctGioHangRepository;
        private readonly UserManager<ThongTinNguoiDung> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<ShoppingCartController> _logger; // Thêm ILogger

        // Constructor: Nhận các dịch vụ/repository qua dependency injection để sử dụng
        public ShoppingCartController(
            ApplicationDbContext context,
            ISanPhamRepository productRepository,
            IGioHangRepository luuGioHangRepository,
            ICTGioHangRepository ctGioHangRepository,
            UserManager<ThongTinNguoiDung> userManager,
            IVnPayService vnPayService,
            ILogger<ShoppingCartController> logger)
        {
            _productRepository = productRepository;
            _luuGioHangRepository = luuGioHangRepository;
            _ctGioHangRepository = ctGioHangRepository;
            _userManager = userManager;
            _context = context;
            _vnPayService = vnPayService;
            _logger = logger;
        }

        /*-------------------------------------------------------------------------------------------------*/
        // Thêm sản phẩm vào giỏ hàng (GET)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpGet]
        public IActionResult AddToCart(string returnUrl = null)
        {
            // Chưa đăng nhập: Chuyển hướng đến trang đăng nhập với returnUrl
            string trangTiepTuc = returnUrl ?? "/Home/Index";
            string trangDangNhap = "/Identity/Account/Login?returnUrl=";
            string duongDanDayDu = trangDangNhap + Uri.EscapeDataString(trangTiepTuc);
            return Redirect(duongDanDayDu);
        }


        // Thêm sản phẩm vào giỏ hàng (post)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {


            // Lấy sản phẩm từ cơ sở dữ liệu dựa trên productId
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Lấy thông tin người dùng và khách hàng từ database
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == user.Id);

            // Lấy hoặc tạo giỏ hàng cho khách hàng
            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);

            // Kiểm tra xem sản phẩm đã có trong giỏ chưa
            var ctGioHang = await _ctGioHangRepository.GetCartItemAsync(luuGioHang.MaGH, productId);
            if (ctGioHang != null)
            {
                // Nếu đã có: tăng số lượng và cập nhật giá, thời gian
                ctGioHang.SoLuongThem += 1;
                ctGioHang.ThoiGianThemCuoi = DateTime.Now;
                ctGioHang.GiaThemCuoi = product.Gia;
                await _ctGioHangRepository.UpdateCartItemAsync(ctGioHang);
            }
            else
            {
                // Nếu chưa có: thêm mới sản phẩm vào giỏ hàng
                ctGioHang = new CTGioHang
                {
                    MaGH = luuGioHang.MaGH,
                    MaSP = productId,
                    GiaLucThem = product.Gia,
                    GiaThemCuoi = product.Gia,
                    ThoiGianThemDau = DateTime.Now,
                    ThoiGianThemCuoi = DateTime.Now,
                    SoLuongThem = 1,
                    TrangThaiChon = false
                };
                await _ctGioHangRepository.AddCartItemAsync(ctGioHang);
            }

            // Lấy tổng số lượng sản phẩm trong giỏ hàng


            
            var cartItems = await _ctGioHangRepository.GetCartItemsAsync(luuGioHang.MaGH);
            var totalQuantity = cartItems.Sum(item => item.SoLuongThem);
            return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!", cartCount = totalQuantity });
        }


        /*-------------------------------------------------------------------------------------------------*/
        // Hiển thị giỏ hàng (GET)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra xem đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Đường dẫn đến trang thanh toán (nơi quay lại sau)
                string trangTiepTuc = "/ShoppingCart/Index?returnUrl=";

                // Đường dẫn đến trang đăng nhập
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";

                // Nói rõ rằng sau khi đăng nhập thì quay lại trang thanh toán
                string duongDanDayDu = trangDangNhap + trangTiepTuc;

                // Chuyển người dùng đến trang đăng nhập
                return Redirect(duongDanDayDu);
            }

            // Lấy thông tin người dùng và khách hàng
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return View(new ShoppingCart()); // Trả về giỏ trống nếu không có khách hàng
            }

            // Lấy giỏ hàng và danh sách sản phẩm trong giỏ
            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);
            var cartItems = await _ctGioHangRepository.GetCartItemsAsync(luuGioHang.MaGH);

            // Nếu giỏ trống, trả về view với giỏ rỗng
            if (cartItems == null || !cartItems.Any())
            {
                return View(new ShoppingCart());
            }

            // Chuyển dữ liệu từ CTGioHang sang ShoppingCart để hiển thị
            var cart = new ShoppingCart
            {
                Items = cartItems.Select(ct =>
                {
                    var product = _context.SanPham.FirstOrDefault(p => p.MaSP == ct.MaSP);
                    return new CartItem
                    {
                        ProductId = ct.MaSP,
                        Name = ct.SanPham.TenSP,
                        Price = product?.Gia ?? ct.GiaThemCuoi, // Giá hiện tại từ Product, nếu null thì dùng giá cũ
                        Quantity = ct.SoLuongThem,
                        IsSelected = ct.TrangThaiChon,
                        ImageUrl = product?.UrlAnh ?? (product?.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/50x50")
                    };
                }).ToList()
            };

            // Trả về view với dữ liệu giỏ hàng
            return View(cart);
        }


        /*-------------------------------------------------------------------------------------------------*/
        

        private async Task RemoveCartItemAsync(int maGH, int productId)
        {
            await _ctGioHangRepository.DeleteCartItemAsync(maGH, productId);
        }

        // Xóa nhiều sản phẩm được chọn (Post)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSelectedFromCart()
        {
            // Kiểm tra trạng thái đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                string trangTiepTuc = "/ShoppingCart/Index?returnUrl=";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Redirect(duongDanDayDu);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Index");
            }

            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);
            var cartItems = await _ctGioHangRepository.GetCartItemsAsync(luuGioHang.MaGH);
            var selectedItems = cartItems.Where(ct => ct.TrangThaiChon).ToList();

            if (!selectedItems.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để xóa.";
                return RedirectToAction("Index");
            }
            foreach (var item in selectedItems)
            {
                await RemoveCartItemAsync(luuGioHang.MaGH, item.MaSP);
            }
            TempData["Message"] = "Đã xóa các sản phẩm được chọn.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSingleFromCart(string productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                string trangTiepTuc = "/ShoppingCart/Index?returnUrl=";
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";
                string duongDanDayDu = trangDangNhap + trangTiepTuc;
                return Redirect(duongDanDayDu);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Index");
            }

            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);
            var cartItems = await _ctGioHangRepository.GetCartItemsAsync(luuGioHang.MaGH);
            var itemToRemove = cartItems.FirstOrDefault(ct => ct.MaSP == int.Parse(productId));

            if (itemToRemove == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại trong giỏ hàng.";
                return RedirectToAction("Index");
            }

            await RemoveCartItemAsync(luuGioHang.MaGH, itemToRemove.MaSP);
            TempData["Message"] = "Đã xóa sản phẩm được chọn.";
            return RedirectToAction("Index");
        }

        /*-------------------------------------------------------------------------------------------------*/
        // Chuyển đến trang thanh toán cho các sản phẩm được chọn (POST)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutSelectedItems()
        {
            // Kiểm tra xem đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Đường dẫn đến trang thanh toán (nơi quay lại sau)
                string trangThanhToan = "/HoaDon/ThanhToan";

                // Đường dẫn đến trang đăng nhập
                string trangDangNhap = "/Identity/Account/Login?returnUrl=";

                // Nói rõ rằng sau khi đăng nhập thì quay lại trang thanh toán
                string duongDanDayDu = trangDangNhap + trangThanhToan;

                // Chuyển người dùng đến trang đăng nhập
                return Redirect(duongDanDayDu);
            }

            // Lấy thông tin khách hàng
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return RedirectToAction("Index");
            }

            // Lấy danh sách sản phẩm được chọn trong giỏ
            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);
            var selectedItems = await _ctGioHangRepository.GetCartItemsAsync(luuGioHang.MaGH);
            selectedItems = selectedItems.Where(ct => ct.TrangThaiChon).ToList();

            // Nếu không có sản phẩm nào được chọn, thông báo lỗi
            if (!selectedItems.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán.";
                return RedirectToAction("Index");
            }

            // Chuyển dữ liệu sang CartItem để lưu vào Session
            var cartItems = selectedItems.Select(ct =>
            {
                var product = _context.SanPham.FirstOrDefault(p => p.MaSP == ct.MaSP);
                return new CartItem
                {
                    ProductId = ct.MaSP,
                    Name = ct.SanPham.TenSP,
                    Price = product?.Gia ?? ct.GiaThemCuoi, // Giá hiện tại từ Product nếu ngoại lệ lấy giá cuối
                    Quantity = ct.SoLuongThem,
                    IsSelected = ct.TrangThaiChon,
                    ImageUrl = product?.UrlAnh ?? (product?.DSAnh?.FirstOrDefault()?.UrlAnh ?? "https://via.placeholder.com/200x200"),
                    Description = product?.MoTa
                };
            }).ToList();

            // Lưu danh sách sản phẩm được chọn vào Session
            HttpContext.Session.SetString("SelectedCartItems", JsonConvert.SerializeObject(cartItems));

            // Chuyển đến trang thanh toán
            //return RedirectToAction("ThanhToanMultiple", "HoaDon");
            return RedirectToAction("ThanhToanChung", "HoaDon");
        }

        /*-------------------------------------------------------------------------------------------------*/
        // Cập nhật trạng thái chọn hoặc số lượng sản phẩm trong giỏ (POST)
        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        public async Task<IActionResult> UpdateSelection(int productId, bool isSelected, int? quantity = null)
        {
            // Kiểm tra đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            // Lấy thông tin khách hàng
            var user = await _userManager.GetUserAsync(User);
            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.UserId == user.Id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            // Lấy giỏ hàng và sản phẩm trong giỏ
            var luuGioHang = await _luuGioHangRepository.GetOrCreateGioHangAsync(khachHang.MaKH);
            var cartItem = await _ctGioHangRepository.GetCartItemAsync(luuGioHang.MaGH, productId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            // Cập nhật trạng thái chọn
            cartItem.TrangThaiChon = isSelected;

            // Cập nhật số lượng nếu có giá trị
            if (quantity.HasValue)
            {
                if (quantity <= 0)
                {
                    // Nếu số lượng <= 0, xóa sản phẩm
                    await _ctGioHangRepository.DeleteCartItemAsync(luuGioHang.MaGH, productId);
                    return Json(new { success = true, message = "Đã xóa sản phẩm.", removed = true });
                }
                // Cập nhật số lượng mới
                cartItem.SoLuongThem = quantity.Value;
            }

            // Lưu thay đổi vào database
            await _ctGioHangRepository.UpdateCartItemAsync(cartItem);

            // Tính tổng tiền mới dựa trên giá hiện tại
            var product = await _context.SanPham.FirstOrDefaultAsync(p => p.MaSP == productId);
            var newTotal = (product?.Gia ?? cartItem.GiaThemCuoi) * cartItem.SoLuongThem;

            // Trả về JSON với thông tin cập nhật
            return Json(new
            {
                success = true,
                message = "Cập nhật thành công.",
                newTotal = newTotal.ToString("N0"),
                quantity = cartItem.SoLuongThem
            });
        }
    }
}