using System.ComponentModel.DataAnnotations;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    public class ProductImg
    {
        [Key]
        public int ProductImgId { get; set; }
        [StringLength(300)]
        public string? Picture { get; set; }
        public string? ProductId { get; set; }
    }
}
