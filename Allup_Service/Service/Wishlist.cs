using Allup_Core.Entities;

namespace Allup_Service.Service
{
    internal class Wishlist : WishList
    {
        public string AppUserId { get; set; }
        public List<WishListProduct> WishListProducts { get; set; }
    }
}