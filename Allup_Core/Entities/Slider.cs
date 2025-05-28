using Allup_Core.Attributes;
using Allup_Core.Comman;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Allup_Core.Entities
{
    public class Slider:BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(100)]    
        public string Desc { get; set; } =null!;
        public string Image { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile  Photo { get; set; }

    }
}
