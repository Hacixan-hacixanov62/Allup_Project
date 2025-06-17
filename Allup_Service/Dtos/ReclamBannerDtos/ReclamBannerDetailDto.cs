
using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.ReclamBannerDtos
{
    public class ReclamBannerDetailDto
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
