
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.CartDtos
{
    public class CartDetailDto
    {
        public int Id { get; set; }
        public int Count { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Subtotal Duzgun daxil edin.")]
        public decimal Subtotal { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Discount Duzgun daxil edin.")]
        public decimal Discount { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Total Duzgun daxil edin.")]
        public decimal Total { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name Duzgun daxil edin.")]
        public string Name { get; set; } = null!;
        public string MainImage { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "Price Duzgun daxil edin.")]
        public decimal Price { get; set; }
    }
}
