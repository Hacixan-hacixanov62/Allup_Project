

using Allup_Service.Dtos.CartDtos;

namespace Allup_Service.Service.IService
{
    public interface IBasketService
    {
        Task<bool> AddToCartAsync(int id, int count = 1);
        Task<bool> DecreaseToCartAsync(int id);
        Task RemoveToCartAsync(int id);
        Task ClearCartAsync();

        Task RemoveToBasket(int id, string? returnUrl);
        Task EditBasketItem(int id, int count);
        Task AddToBasket(int id, string? returnUrl, int count = 1, int page = 1);
        CartGetDto GetUserBasketItem();
        Task DeleteBasket(int id);
        Task<CartGetDto> GetCartAsync();
        Task<int> GetIntAsync();
    }
}
