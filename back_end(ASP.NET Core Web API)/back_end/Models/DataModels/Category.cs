using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string? CategoryName { get; set; }
        [Required]
        public bool Status { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
