using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class BlogTag:BaseEntity
    {
        public int BlogId { get; set; }
        public Blog Blog { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
