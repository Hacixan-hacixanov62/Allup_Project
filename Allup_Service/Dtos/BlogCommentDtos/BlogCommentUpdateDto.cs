using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.BlogCommentDtos
{
    public class BlogCommentUpdateDto:IDto
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int Rating { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
