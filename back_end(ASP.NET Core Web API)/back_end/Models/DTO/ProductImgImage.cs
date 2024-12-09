using System.ComponentModel.DataAnnotations;

namespace back_end.Models.DTO
{
    public class ProductImgImage
    {
        public int ProductImgId { get; set; }
        [StringLength(300)]
        public IFormFile? Picture { get; set; }
        public string? ProductId { get; set; }
    }
}
