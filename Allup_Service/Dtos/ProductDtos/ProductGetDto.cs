using Allup_Core.Attributes;
using Allup_Core.Entities;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;
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
    public class ProductGetDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(240)]
        public string Desc { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price mənfi ola bilməz.")]
        public decimal CostPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price mənfi ola bilməz.")]
        public decimal SalePrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Discount Duzgun daxil edin.")]
        public decimal DiscountPercent { get; set; }
        public int StockCount { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }
        public bool IsNew { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFeatured { get; set; }

        public string MainFileImage { get; set; } = null!;
        public string MainFileUrl { get; set; } = null!;
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile MainFile { get; set; } = null!;
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public List<IFormFile> AdditionalFiles { get; set; } = new();
        public List<ProductImage> ProductImages { get; set; }
        public List<int>? ProductImageIds { get; set; }

        public List<CategoryGetDto> Categories { get; set; } = null!;
        public List<TagGetDto> Tags { get; set; } = null!;
        public List<ColorGetDto> Colors { get; set; } = null!;
        public List<SizeGetDto> Sizes { get; set; } = null!;

        public List<ProductGetDto> Products { get; set; } = [];

        //Bunu baskete gore yazmisam sile bilerem


    }
}
