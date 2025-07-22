using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.CommentDtos
{
    public class CommentUpdateDto:IDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        //public int BlogId { get; set; }
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
