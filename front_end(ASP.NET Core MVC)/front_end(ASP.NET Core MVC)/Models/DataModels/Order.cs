
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    public class Order
    {
        [Key]
        [Column(TypeName = "varchar(6)")]
        [StringLength(6, ErrorMessage = "Order ID must be exactly 6 characters.")]
        [Required(ErrorMessage = "Order ID is required.")]
        public string? OrderId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Receiver must be between 3 and 200 characters.")]
        public string? Receiver { get; set; }

        [DisplayName("Phone Number")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid phone number.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 250 characters.")]
        public string? Address { get; set; }

        public double ToltalPrice { get; set; }

        [StringLength(300, MinimumLength = 0, ErrorMessage = "Note cannot exceed 300 characters.")]
        public string? Note { get; set; }

        public byte Status { get; set; }

        [DisplayName("Created At")]
        public DateTime Created_at { get; set; }

        public int AccountId { get; set; }

        public Account? Account { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
