using Allup_Core.Comman;
using System.ComponentModel.DataAnnotations;

namespace Allup_Core.Entities
{
    public class Tag:BaseAuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public List<Product> Products { get; set; } =null!;
        public List<BlogTag> BlogTags { get; set; } = new List<BlogTag>();

    }
}
