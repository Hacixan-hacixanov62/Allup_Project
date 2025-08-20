using Allup_Core.Entities;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.WisListDtos;

namespace Allup_Service.Service.IService
{
    public interface IWishListService
    {
        //int AddWishlist(int id, ProductGetDto product);
        //int GetCount();
        //Task<List<WislistDetailDto>> GetWishlistDatasAsync();
        //void DeleteItem(int id);
        //List<WishListDto> GetDatasFromCookies();
        //void SetDatasToCookies(List<WishListDto> wishlist, Product dbProduct, WishListDto existProduct);
        //Task<WishListDto> GetByUserIdAsync(string userId);
        //Task<List<WishlistItem>> GetAllByWishlistIdAsync(int? wishlistId);
        Task<bool> AddToWishListAsync(int id,int count =1);
        Task<int> WishlistCount();
        Task<WishListCardDto> WishListCardVM();
        Task<bool> RemoveFromWishListAsync(int id);
        Task<bool> DecreaseToCartAsync(int id);
        Task<List<WishlistItem>> GetWishListAsync();
        Task<int> GetIntAsync();

    }
}
