using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("ProductImgs")]
    public class ProductImg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductImgId { get; set; }
        [StringLength(300)]
        public string? Picture { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; } = null!;
        public Product? Product { get; set; }
    }
}
