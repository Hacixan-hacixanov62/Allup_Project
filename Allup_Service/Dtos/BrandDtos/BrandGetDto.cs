

using Allup_Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.BrandDtos
{
    public class BrandGetDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}
