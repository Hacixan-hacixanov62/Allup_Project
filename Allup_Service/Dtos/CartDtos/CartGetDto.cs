
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.CartDtos
{
    public class CartGetDto
    {
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

        public List<CartItemDto> Items { get; set; } = [];
    }
}
