

using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Service.Dtos.BlogDtos
{
    public class BlogCreateDto
    {

        [Required]
        [StringLength(maximumLength: 150)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 500)]
        public string MinDescription { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 500)]
        public string MaxDescription { get; set; } = null!;
        public int AuthorId { get; set; }
        public int TagId { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile Image { get; set; } = null!;
       // public List<int> TopicIds { get; set; } = new();
    }
}
