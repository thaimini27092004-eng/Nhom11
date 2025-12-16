namespace WebsiteBanHang.Models.VnPay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }  // Loại đơn hàng
        public double Amount { get; set; }     // Số tiền thanh toán
        public string OrderDescription { get; set; }  // Mô tả đơn hàng
        public string Name { get; set; }       // Tên khách hàng hoặc đơn hàng
        public int OrderId { get; internal set; }
    }
}
//Class lưu dữ liệu chuyển đổi lên VNPay