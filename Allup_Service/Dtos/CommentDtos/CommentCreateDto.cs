using Allup_Service.Abstractions.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.CommentDtos
{
    public class CommentCreateDto:IDto
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Text Duzgun daxil edin.")]
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
