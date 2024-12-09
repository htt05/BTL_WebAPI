using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [DisplayName("Full Name")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 250 characters.")]
        [Required]
        public string? AccountName { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters.")]
        [Required]
        public string? Email { get; set; }

        [DisplayName("Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }

        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm Password does not match Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Confirm Password must be at least 6 characters.")]
        [Required]
        public string? ConfirmPassWord { get; set; }

        [DisplayName("Phone Number")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid phone number.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Required]
        public string? PhoneNumber { get; set; }

        public byte Role { get; set; }

        public string? Token { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
