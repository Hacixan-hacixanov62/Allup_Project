
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.CommentDtos
{
    public class CommentReplyDto
    {
        public int ParentId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Text Duzgun daxil edin.")]
        public string Text { get; set; } = null!;
    }
}
