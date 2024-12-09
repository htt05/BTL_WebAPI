using System.ComponentModel.DataAnnotations;

namespace front_end_ASP.NET_Core_MVC_.Models.DTO
{
    public class ProductImgImage
    {
        public int ProductImgId { get; set; }
        [StringLength(300)]
        public IFormFile? Picture { get; set; }
        public string? ProductId { get; set; }
    }
}
