namespace WebsiteBanHang.Models.VnPay
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; }   // Mô tả đơn hàng
        public string TransactionId { get; set; }       // ID giao dịch
        public string OrderId { get; set; }             // ID đơn hàng
        public string PaymentMethod { get; set; }       // Phương thức thanh toán
        public string PaymentId { get; set; }           // ID thanh toán
        public bool Success { get; set; }               // Trạng thái thanh toán (thành công hay không)
        public string Token { get; set; }               // Token bảo mật (dùng trong giao dịch)
        public string VnPayResponseCode { get; set; }   // Mã phản hồi từ VNPay
    }
}

//Nhận lưu thông tin trạng thái sau khi thanh toán của VNPay