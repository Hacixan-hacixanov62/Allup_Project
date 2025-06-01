

using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.BannerDtos
{
    public class BannerUpdateDto
    {
        public bool IsActivated { get; set; }
        [Required]
        [StringLength(200)]
        public string RedirectUrl { get; set; } = null!;
        public string ImageUrl { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile? ImageFile { get; set; }
    }
}
