

using Allup_Service.Dtos.CartDtos;
using Allup_Service.Dtos.WisListDtos;

namespace Allup_Service.UI.Services.IServices
{
    public interface ILayoutService
    {
        CartGetDto GetUserBasketItem();
        WishListCookieItemDto GetWishListItem();
    }
}
