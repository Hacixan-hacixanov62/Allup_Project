using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class Comment:BaseAuditableEntity
    {
        public string Text { get; set; } = null!;
        public string? AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;
        public int? ProductId { get; set; }
        public Product? Product { get; set; } = null!;
        public int Rating { get; set; }
        public int? ParentId { get; set; }
        //[NotMapped] 
        //public double AverageRating { get; set; }
        public Comment? Parent { get; set; } = null!;
        public ICollection<Comment> Children { get; set; } = [];
    }
}
