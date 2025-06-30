using Allup_DataAccess.DAL;
using Allup_Service.Dtos.CartDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.UI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Allup_Service.UI.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public CartGetDto GetUserBasketItem()
        {
            CartGetDto cartGetDto = new CartGetDto();

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var items = _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages.Where(x => x.IsCover == true))
                    .Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {

                    CartItemDto basketItemVM = new CartItemDto()
                    {
                        Count = bi.Count,
                        MainImage = bi.Product.ProductImages.FirstOrDefault(m => m.IsCover == true).ImageUrl,
                        Product = new ProductGetDto
                        {
                            Id = bi.Product.Id,
                            Name = bi.Product.Name,
                            CostPrice = bi.Product.CostPrice,
                            SalePrice = bi.Product.SalePrice,
                            DiscountPercent = bi.Product.DiscountPercent
                        }
                    };
                    // ProductGetDto dto = _mapper.Map<ProductGetDto>(bi.Product);

                    cartGetDto.Items.Add(basketItemVM);
                    cartGetDto.Total += (basketItemVM.Product.DiscountPercent > 0 ? basketItemVM.Product.DiscountPercent : basketItemVM.Product.CostPrice) * basketItemVM.Count;

                }

            }
            else
            {
                var basketStr = _httpContextAccessor.HttpContext.Request.Cookies["AllupCart"];

                List<CartItemCreateDto> cookieItems = null;

                if (basketStr != null)
                    cookieItems = JsonConvert.DeserializeObject<List<CartItemCreateDto>>(basketStr);
                else
                    cookieItems = new List<CartItemCreateDto>();


                try
                {
                    foreach (var cItem in cookieItems)
                    {
                        var product = _context.Products.Include(x => x.ProductImages.Where(x => x.IsCover == true)).FirstOrDefault(x => x.Id == cItem.ProductId);

                        if (product is null)
                            continue;

                        CartItemDto basketItemVM = new CartItemDto()
                        {

                            Count = cItem.Count,
                            MainImage = product.ProductImages.FirstOrDefault(x => x.IsCover == true)?.ImageUrl,
                            Product = new ProductGetDto
                            {
                                Id = product.Id,
                                Name = product.Name,
                                CostPrice = product.CostPrice,
                                SalePrice = product.SalePrice,
                                MainFileImage = product.ProductImages.FirstOrDefault(x => x.IsCover == true)?.ImageUrl,
                            }
                        };


                        cartGetDto.Items.Add(basketItemVM);
                        cartGetDto.Total += (basketItemVM.Product.DiscountPercent > 0 ? basketItemVM.Product.DiscountPercent : basketItemVM.Product.CostPrice) * basketItemVM.Count;

                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }



            }

            return cartGetDto;
        }
    }
}
