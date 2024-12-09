using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public string? OrderId { get; set; }
        public Order? Order { get; set; }
        public string? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
