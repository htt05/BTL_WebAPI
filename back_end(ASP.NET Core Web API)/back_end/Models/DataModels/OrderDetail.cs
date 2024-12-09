using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }
        [ForeignKey("Product")]
        [StringLength(6)]
        public string? ProductId { get; set; }
        [ForeignKey("Order")]
        public string? OrderId { get; set; }
        public int Quantity { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
