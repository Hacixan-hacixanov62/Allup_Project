using Allup_Service.Abstractions.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.BlogCommentDtos
{
    public class BlogCommentCreateDto:IDto
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
    }
}
