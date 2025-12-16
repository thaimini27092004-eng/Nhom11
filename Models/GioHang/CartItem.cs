namespace WebsiteBanHang.Models.GioHang
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }//ánh xạ TrangThaiChon để lọc danh sách
        public string ImageUrl { get; set; } // Thêm thuộc tính hình ảnh
        public string Description { get; set; } // Thêm thuộc tính mô tả
    }
}