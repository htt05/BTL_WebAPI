﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_end_ASP.NET_Core_MVC_.Models.DataModels
{
    public class Product
    {
        [Key]
        [Column(TypeName = "varchar(6)")]
        [StringLength(6, ErrorMessage = "Product ID must be exactly 6 characters.")]
        [Required(ErrorMessage = "Product ID is required.")]
        public string? ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 200 characters.")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; }

        public double? SalePrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        [Required(ErrorMessage = "Discount is required.")]
        public int Discount { get; set; } = 0;
        [Required(ErrorMessage = "Product Count is required.")]
        public int ProductCount { get; set; } = 0;

        [StringLength(300, ErrorMessage = "Picture URL cannot exceed 300 characters.")]
        public string? Picture { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public byte Status { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
        public ICollection<ProductImg>? ProductImgs { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}