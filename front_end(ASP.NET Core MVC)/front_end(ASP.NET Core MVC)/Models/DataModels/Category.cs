using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Category Name must be between 3 and 100 characters.")]
        [DisplayName("Category Name")]
        public string? CategoryName { get; set; }
        [Required]
        public bool Status { get; set; }
    }
}
