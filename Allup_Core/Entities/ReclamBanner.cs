

using Allup_Core.Attributes;
using Allup_Core.Comman;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Core.Entities
{
    public class ReclamBanner:BaseAuditableEntity
    {
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        [Required]
        [StringLength(200)]
        public string RedirectUrl { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile? ImageFile { get; set; }
    }
}
