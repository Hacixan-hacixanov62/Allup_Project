using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Dtos.CartDtos;
using Allup_Service.Dtos.CompareDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.WisListDtos;
using Allup_Service.Service;
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

        public CompareDetailDto GetCompareItem()
        {
            CompareDetailDto wishListDto = new CompareDetailDto();

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var items = _context.Compares
                    .Include(m => m.Product)
                    .ThenInclude(m => m.ProductImages.Where(pi => pi.IsCover == true))
                    .Where(m => m.AppUserId == userId)
                    .ToList();

                foreach (var bi in items)
                {
                    var wishlistItem = new CompareItemCard
                    {
                        ProductId = bi.Product.Id,
                        Name = bi.Product.Name,
                        Price = bi.Product.CostPrice,
                        Count = bi.Count,
                        Desc = bi.Product.Desc,
                        InStock = bi.Product.StockCount,
                        //Color = bi.Product.ColorProducts.Select(cp => cp.Color.Name).ToList()
                    };

                    wishListDto.CompareItems.Add(wishlistItem);
                }
            }
            else
            {
                var basketStr = _httpContextAccessor.HttpContext.Request.Cookies[CompareService.COMPARE_KEY];
                List<CompareDetailDto> cookieItems = null;

                if (basketStr != null)
                    cookieItems = JsonConvert.DeserializeObject<List<CompareDetailDto>>(basketStr);
                else
                    cookieItems = new List<CompareDetailDto>();

                foreach (var cItem in cookieItems)
                {
                    var wishlistItem = new CompareItemCard
                    {
                        ProductId = cItem.ProductId,
                        Name = cItem.Name,
                        Price = cItem.Price,
                        Count = cItem.Count,
                        Desc = cItem.Desc,
                        InStock = cItem.InStock,
                    };

                    wishListDto.CompareItems.Add(wishlistItem);
                }
            }

            return wishListDto;
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

        public WislistDetailDto GetWishListItem()
        {
            WislistDetailDto wishListDto = new WislistDetailDto();

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var items = _context.WishlistItems
                    .Include(m => m.Product)
                    .ThenInclude(m => m.ProductImages.Where(pi => pi.IsCover == true))
                    .Where(m => m.AppUserId == userId)
                    .ToList();

                foreach (var bi in items)
                {
                    var wishlistItem = new WishlistItemCard
                    {
                        ProductId = bi.Product.Id,
                        Name = bi.Product.Name,
                        Price = bi.Product.CostPrice,
                        Count = bi.Count
                    };

                    wishListDto.WishlistItems.Add(wishlistItem);
                }
            }
            else
            {
                var basketStr = _httpContextAccessor.HttpContext.Request.Cookies[WishListService.WishList_KEY];
                List<WislistDetailDto> cookieItems = null;

                if (basketStr != null)
                    cookieItems = JsonConvert.DeserializeObject<List<WislistDetailDto>>(basketStr);
                else
                    cookieItems = new List<WislistDetailDto>();

                foreach (var cItem in cookieItems)
                {
                    var wishlistItem = new WishlistItemCard
                    {
                        ProductId = cItem.ProductId,
                        Name = cItem.Name,
                        Price = cItem.Price,
                        Count = cItem.Count
                    };

                    wishListDto.WishlistItems.Add(wishlistItem);
                }
            }

            return wishListDto;
        }

    }
}
