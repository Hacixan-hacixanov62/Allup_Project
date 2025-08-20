using Allup_Core.Entities;
using Allup_Service.Abstractions.Dtos;
using Allup_Service.Dtos.AppUserDtos;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.BlogCommentDtos
{
    public class BlogCommentGetDto:IDto
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Text Duzgun daxil edin.")]
        public string Text { get; set; } = null!;
        public UserGetDto AppUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<BlogCommentGetDto> Children { get; set; } = new();

    }
}
