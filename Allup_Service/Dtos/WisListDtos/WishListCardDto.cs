using Allup_Core.Entities;

namespace Allup_Service.Dtos.WisListDtos
{
    public class WishListCardDto
    {
        public List<WishlistItemCard> Product { get; set; } = new List<WishlistItemCard>();
    }
}
