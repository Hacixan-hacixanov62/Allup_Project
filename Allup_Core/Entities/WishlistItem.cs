using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class WishlistItem:BaseEntity
    {
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public AppUser AppUser { get; set; } = null!;
        public string AppUserId { get; set; } = null!;
        public int Count { get; set; }

    }
}
