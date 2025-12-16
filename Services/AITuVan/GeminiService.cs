using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories.I;

namespace WebsiteBanHang.Services.AITuVan
{
    public class GeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly ISanPhamRepository _sanPhamRepository;
        private readonly IDanhMucRepository _danhMucRepository;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration, ISanPhamRepository sanPhamRepository, IDanhMucRepository danhMucRepository)
        {
            _configuration = configuration;
            _sanPhamRepository = sanPhamRepository;
            _danhMucRepository = danhMucRepository;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["GrokApi:ApiKey"]}");
        }

        public async Task<string> GetGrokResponseAsync(string userMessage)
        {
            // Lấy dữ liệu sản phẩm và danh mục
            var sanPhams = await _sanPhamRepository.GetAllAsync();
            var danhMucs = await _danhMucRepository.GetAllAsync();

            // Tạo context với hướng dẫn định dạng phản hồi
            StringBuilder context = new StringBuilder();
            // Tạo context với hướng dẫn trả lời thân thiện
            context.AppendLine("Bạn tên là LaLa, trợ lý AI thân thiện của cửa hàng bán hàng online. Nếu câu hỏi không liên quan đến sản phẩm, hãy trả lời một cách lịch sự, vui vẻ và gợi ý người dùng hỏi về sản phẩm nếu cần thiết.");
            context.AppendLine("Nhiệm vụ của bạn là trả lời các câu hỏi liên quan đến sản phẩm và danh mục trong cửa hàng một cách tự nhiên và thân thiện, 1 cuộc trò chuyện với khách hàng có thể có nhiều tin nhắn vì thế phải hiểu cả cuộc trò chuyện chứ không phải chỉ hiểu tin mới nhất");
            context.AppendLine("Ví dụ: Nếu người dùng nói 'Hi bạn', hãy trả lời: 'Chào bạn! Rất vui được trò chuyện, bạn muốn tìm hiểu về sản phẩm nào hôm nay?'");
            context.AppendLine("Khi đề xuất hoặc trả lời về sản phẩm, sử dụng định dạng sau cho mỗi sản phẩm: [PRODUCT]maSP:{MaSP}|tenSP:{TenSP}|gia:{Gia}|danhMuc:{TenDM}|urlAnh:{UrlAnh}[/PRODUCT].");
            context.AppendLine("Ví dụ: [PRODUCT]maSP:1|tenSP:iPhone 14|gia:20000000|danhMuc:Điện thoại|urlAnh:/images/anhchinhsanpham/iphone14.jpg[/PRODUCT]");
            context.AppendLine("Nếu trả lời nhiều sản phẩm, mỗi sản phẩm nằm trong cặp [PRODUCT]...[/PRODUCT]. Đừng thêm thông tin ngoài định dạng này trừ khi cần giải thích thêm, ngoài ra phải dùng dữ liệu sản phẩm, danh mục trong tin nhắn mới nhất thì mới chính xác.");
            context.AppendLine("Hãy sử dụng giọng điệu thân thiện, tự nhiên, như một người bạn đang tư vấn, chính xác trọng tâm câu hỏi, và tránh trả lời khô khan hoặc máy móc.");

            context.AppendLine("Danh mục sản phẩm:");
            foreach (var dm in danhMucs)
            {
                context.AppendLine($"- {dm.TenDM} (Mã: {dm.MaDM})");
            }

            context.AppendLine("Sản phẩm trong cửa hàng:");
            foreach (var sp in sanPhams)
            {
                var urlAnh = !string.IsNullOrEmpty(sp.UrlAnh)? sp.UrlAnh: "/images/anhchinhsanpham/default.jpg";
                context.AppendLine($"- Mã sản phẩm = {sp.MaSP}, Tên sản phẩm= {sp.TenSP}, Giá sản phẩm : {sp.Gia} VND, Danh mục : {sp.DanhMuc?.TenDM ?? "Không có danh mục"}, Ảnh: {urlAnh}");
            }

            // Chuẩn bị yêu cầu gửi đến Gemini API
            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = context.ToString() }, // System message
                    new { text = userMessage } // User message
                }
            }
        },
                generationConfig = new
                {
                    temperature = 1.5
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var apiKey = _configuration["GeminiApi:ApiKey"];
            var response = await _httpClient.PostAsync($"{_configuration["GeminiApi:BaseUrl"]}/models/gemini-1.5-flash:generateContent?key={apiKey}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                return jsonResponse.candidates[0].content.parts[0].text.ToString();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var statusCode = response.StatusCode;
                return $"Lỗi kết nối với AI. Mã trạng thái: {statusCode}. Chi tiết: {errorContent}";
            }
        }
    }
}