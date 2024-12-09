using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.DataModels
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column(TypeName = "varchar(6)")]
        [StringLength(6)]
        [Required]
        public string? OrderId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string? Receiver { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string? Address { get; set; }

        public double ToltalPrice { get; set; }

        [Column(TypeName = "nvarchar(300)")]
        [StringLength(300, MinimumLength = 0)]
        public string? Note { get; set; }

        public byte Status { get; set; }

        public DateTime Created_at { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public Account? Account { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
