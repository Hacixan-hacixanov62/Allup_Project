using Allup_Core.Attributes;
using Allup_Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Dtos.ProductDtos
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(240)]
        public string Desc { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public int StockCount { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }
        public bool IsNew { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFeatured { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile MainFile { get; set; } = null!;
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public List<IFormFile> AdditionalFiles { get; set; } = new();
        public List<ProductImage>? ProductImages { get; set; }
        public List<int>? ProductImageIds { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int BrandId { get; set; }
        public Brand? Brands { get; set; }

        // Many to Many 
        public List<TagProduct> TagProducts { get; set; } = new();
        public List<SizeProduct> SizeProducts { get; set; } = new();
        public List<ColorProduct> ColorProducts { get; set; } = new();
        public List<int> SizeIds { get; set; } = new();
        public List<int> ColorIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
    }
}
