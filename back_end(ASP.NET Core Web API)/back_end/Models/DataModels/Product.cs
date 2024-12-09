using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column(TypeName = "varchar(6)")]
        [StringLength(6)]
        [Required]
        public string? ProductId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string? ProductName { get; set; }

        [Required]
        public double Price { get; set; }

        public double? SalePrice { get; set; }

        [Range(0, 100)]
        public int? Discount { get; set; }

        public int ProductCount { get; set; } = 0;

        [StringLength(300)]
        public string? Picture { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        [Required]
        public byte Status { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<ProductImg>? ProductImgs { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
