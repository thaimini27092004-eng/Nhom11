using WebsiteBanHang.Libraries;
using WebsiteBanHang.Models.VnPay;

namespace WebsiteBanHang.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"]; // Sửa key cho đúng

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(urlCallBack) || model == null || model.Amount <= 0)
            {
                throw new ArgumentException("Dữ liệu đầu vào không hợp lệ.");
            }

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"] ?? "2.1.0");
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"] ?? "pay");
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"] ?? throw new ArgumentNullException("TmnCode is missing"));
            pay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"] ?? "VND");
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"] ?? "vn");
            pay.AddRequestData("vnp_OrderInfo", Uri.EscapeDataString($"{model.Name} {model.OrderDescription}"));
            pay.AddRequestData("vnp_OrderType", model.OrderType ?? "order");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            //pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
            pay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());
            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            Console.WriteLine($"Payment URL: {paymentUrl}"); // Log để kiểm tra
            return paymentUrl;
        }



        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            return response;
        }
    }
}
