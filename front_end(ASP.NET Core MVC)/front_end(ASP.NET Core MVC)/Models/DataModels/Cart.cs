namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    public class Cart
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public int Quantity { get; set; } = 1;
        public string ProductId { get; set; }
        public double? Price { get; set; }
        public double? Total { get { return Quantity * Price; } }
    }
}
