
using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class CartItem:BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
