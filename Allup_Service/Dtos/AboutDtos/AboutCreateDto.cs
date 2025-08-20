using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.AboutDtos
{
    public class AboutCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(20, ErrorMessage = "Title cannot exceed 20 characters")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Desc { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile ImageFile { get; set; } = null!;
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(20, ErrorMessage = "Title cannot exceed 20 characters")]
        public string Title1 { get; set; } = null!;
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(20, ErrorMessage = "Title cannot exceed 20 characters")]
        public string Title2 { get; set; } = null!;
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(20, ErrorMessage = "Title cannot exceed 20 characters")]
        public string Title3 { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Desc1 { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Desc2 { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Desc3 { get; set; } = null!;
    }
}
