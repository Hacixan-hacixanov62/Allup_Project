using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class WishList:BaseAuditableEntity
    {
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public List<WishListProduct> WishListProducts { get; set; }
    }
}
