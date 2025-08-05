using Allup_Core.Entities;
using Allup_Service.Abstractions.Dtos;
using Allup_Service.Dtos.AppUserDtos;

namespace Allup_Service.Dtos.BlogCommentDtos
{
    public class BlogCommentGetDto:IDto
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int Rating { get; set; }
        public string Email { get; set; } = null!;
        public string Text { get; set; } = null!;
        public UserGetDto AppUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<BlogComment> Children { get; set; } = [];

    }
}
