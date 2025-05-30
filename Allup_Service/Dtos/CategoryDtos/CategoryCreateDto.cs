

using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.CategoryDtos
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public bool IsActivated { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile ImageFile { get; set; }
    }
}
