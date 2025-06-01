using Allup_Core.Comman;
using System.ComponentModel.DataAnnotations;

namespace Allup_Core.Entities
{
    public class Brand:BaseAuditableEntity
    {
        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}
