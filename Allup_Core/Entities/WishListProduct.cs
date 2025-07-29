using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class WishListProduct:BaseAuditableEntity
    {
        public int ProductId { get; set; }
        public int WishlistId { get; set; }
        public Product Product { get; set; }
        public WishList Wishlist { get; set; }
    }
}
