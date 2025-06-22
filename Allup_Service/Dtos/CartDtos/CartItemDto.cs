

using Allup_Service.Dtos.ProductDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.CartDtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public ProductGetDto Product { get; set; } = null!;

        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name Duzgun daxil edin.")]
        public string Name { get; set; } = null!;
        public string MainImage { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Price Duzgun daxil edin.")]
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
