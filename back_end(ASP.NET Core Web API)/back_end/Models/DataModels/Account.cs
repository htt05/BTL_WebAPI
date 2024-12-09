using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string? AccountName { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(250)]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        public byte Role { get; set; }

        [NotMapped]
        public string? Token { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
