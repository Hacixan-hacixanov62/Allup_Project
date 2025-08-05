using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class WishlistItemCard
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public int Count { get; set; }

    }
}
