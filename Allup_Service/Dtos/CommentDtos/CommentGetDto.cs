using Allup_Service.Abstractions.Dtos;
using Allup_Service.Dtos.AuthDtos;
using System.ComponentModel.DataAnnotations;


namespace Allup_Service.Dtos.CommentDtos
{
    public class CommentGetDto:IDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        //public int BlogId { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Text Duzgun daxil edin.")]
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public UserGetDto? AppUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentGetDto> Children { get; set; } = [];
    }
}
