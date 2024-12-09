using back_end.Models.DataModels;

namespace back_end.Models.DTO
{
    public class Cart
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; } = 1;
        public double? Total { get { return Quantity * Product.SalePrice; } }
    }
}
