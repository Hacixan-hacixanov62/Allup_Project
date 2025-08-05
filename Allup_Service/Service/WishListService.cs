using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.WisListDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public const string WishList_KEY = "wishlist";

        public WishListService(IWishListRepository wishListRepository, AppDbContext context, IProductService productService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _wishListItemRepository = wishListRepository;
            _context = context;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<bool> AddToWishListAsync(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found!");

            var user = _httpContextAccessor.HttpContext!.User;
            var isAuthenticated = user.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var existingItem = await _wishListItemRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);
                if (existingItem != null)
                {
                    await _wishListItemRepository.Delete(existingItem);
                    await _wishListItemRepository.SaveChangesAsync();
                }
                else
                {
                    WishlistItem wishListItem = new WishlistItem
                    {
                        AppUserId = userId,
                        ProductId = id
                    };
                    await _wishListItemRepository.CreateAsync(wishListItem);
                    await _wishListItemRepository.SaveChangesAsync();
                }
            }
            else
            {
                var cookies = _httpContextAccessor.HttpContext.Response.Cookies;
                var requestCookies = _httpContextAccessor.HttpContext.Request.Cookies;

                List<WishListCookieItemDto> wishList = new List<WishListCookieItemDto>();

                string? cookieData = requestCookies[WishList_KEY];
                if (!string.IsNullOrEmpty(cookieData))
                {
                    wishList = JsonConvert.DeserializeObject<List<WishListCookieItemDto>>(cookieData) ?? new List<WishListCookieItemDto>();
                }

                var existingCookieItem = wishList.FirstOrDefault(x => x.ProductId == id);
                if (existingCookieItem != null)
                {
                    wishList.Remove(existingCookieItem);
                }
                else
                {
                    wishList.Add(new WishListCookieItemDto
                    {
                        ProductId = id
                    });
                }

                string updatedCookie = JsonConvert.SerializeObject(wishList);
                cookies.Append(WishList_KEY, updatedCookie, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(14),
                    HttpOnly = false,
                    Secure = false
                });
            }

            return true;
        }

        public async Task<int> WishlistCount()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var wishlistItems = _wishListItemRepository
                    .GetFilter(x => x.AppUserId == userId)
                    .ToList();

                return wishlistItems.Count;
            }
            else
            {
                var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[WishList_KEY];
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    var cookieItems = JsonConvert.DeserializeObject<List<WishListCookieItemDto>>(cookie);
                    return cookieItems?.Count ?? 0;
                }
            }

            return 0;
        }

        // Bu method Index ucun yazmisdim
        public async Task<WishListCardDto> WishListCardVM()
        {
            var result = new WishListCardDto
            {
                Product = new List<WishlistItemCard>()
            };

            var user = _httpContextAccessor.HttpContext?.User;
            var isAuthenticated = user?.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var wishlistItems = _wishListItemRepository
                    .GetFilter(x => x.AppUserId == userId, include: x => x.Include(p => p.Product))
                    .ToList();

                result.Product = wishlistItems.Select(x => new WishlistItemCard
                {
                    ProductId = x.ProductId,
                    Product = x.Product,
                    Name = x.Product?.Name,
                    ImageUrl = x.Product?.MainFileImage,
                    Price = x.Product?.CostPrice ?? 0,  
                    Count = x.Count
                }).ToList();
            }
            else
            {
                var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[WishList_KEY];
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    var cookieItems = JsonConvert.DeserializeObject<List<WishListCookieItemDto>>(cookie);
                    if (cookieItems != null)
                    {
                        foreach (var item in cookieItems)
                        {
                            var product = await _productService.GetAsync(item.ProductId);
                            if (product != null)
                            {
                                result.Product.Add(new WishlistItemCard
                                {
                                    ProductId = product.Id,
                                    Name = product.Name,
                                    ImageUrl = product.MainFileImage,
                                    Price = product.CostPrice,
                                    Count = 1
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<bool> RemoveFromWishListAsync(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found!");

            var user = _httpContextAccessor.HttpContext!.User;
            var isAuthenticated = user.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var wishlistItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(w => w.AppUserId == userId && w.ProductId == id);

                if (wishlistItem != null)
                {
                    _context.WishlistItems.Remove(wishlistItem);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                // Guest user: Remove from cookie
                var cookie = _httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY];
                if (cookie != null)
                {
                    var wishlist = JsonConvert.DeserializeObject<List<WishListCookieItemDto>>(cookie);
                    var item = wishlist.FirstOrDefault(x => x.ProductId == id);
                    if (item != null)
                    {
                        wishlist.Remove(item);
                        string updatedCookie = JsonConvert.SerializeObject(wishlist);
                        _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, updatedCookie);
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DecreaseToCartAsync(int id)
        {
            var isExistProduct = await _productService.IsExistAsync(id);

            if (isExistProduct is false)
                throw new NotFoundException("Notfound Product");

            if (_checkAuthorized())
            {
                string userId = _getUserId();

                var existCartItem = await _wishListItemRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);

                if (existCartItem is null)
                    throw new NotFoundException("Notfound Product");

                if (existCartItem.Count <= 1)
                    return true;

                existCartItem.Count--;

                _wishListItemRepository.Update(existCartItem);
                await _wishListItemRepository.SaveChangesAsync();

                return true;

            }
            else
            {
                var cartItems = _readCartFromCookie();

                var existItem = cartItems.FirstOrDefault(x => x.ProductId == id);

                if (existItem is null)
                    throw new NotFoundException("Notfound Product");

                if (existItem.Count <= 1)
                    return true;

                existItem.Count--;

                _writeCartInCookie(cartItems);

                return true;
            }
        }








        public async Task<List<WishlistItem>> GetWishListAsync()
        {

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = await _context.WishlistItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == userId).ToListAsync();
                return basketItems;
            }

            var basktItms = _getBasket();
            foreach (var item in basktItms)
            {
                var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == item.ProductId);
                item.Product = product;


            }

            return basktItms;
        }


        public async Task<int> GetIntAsync()
        {
            if (_checkAuthorized())
            {
                var userId = _getUserId();
                var count = await _context.WishlistItems
                                          .Where(x => x.AppUserId == userId)
                                          .Select(x => x.ProductId)
                                          .Distinct()
                                          .CountAsync(); // Eyni məhsulu bir dəfə say
                return count;
            }

            var request = _httpContextAccessor.HttpContext.Request;
            if (request.Cookies[WishList_KEY] != null)
            {
                var basketItems = JsonConvert.DeserializeObject<List<WishlistItem>>(request.Cookies[WishList_KEY]);
                var distinctCount = basketItems.Select(x => x.ProductId)
                                               .Distinct()
                                               .Count();
                return distinctCount;
            }

            return 0;
        }

        public List<WishlistItem> _getBasket()
        {
            List<WishlistItem> basketItems = new();
            if (_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<WishlistItem>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]) ?? new();
            }

            return basketItems;
        }

        private void _writeCartInCookie(List<WishlistItem> cartItems)
        {
            string newJson = JsonConvert.SerializeObject(cartItems);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(WishList_KEY, newJson);
        }


        private string _getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private bool _checkAuthorized()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
        private List<WishlistItem> _readCartFromCookie()
        {
            string json = _httpContextAccessor.HttpContext?.Request.Cookies[WishList_KEY] ?? "";

            var cartItems = JsonConvert.DeserializeObject<List<WishlistItem>>(json) ?? new();
            return cartItems;
        }

     


        #region WISHLIST
        //public int AddWishlist(int id, ProductGetDto product)
        //{
        //    // Cookies hissəsi (var)
        //    List<WishListDto> wishlist = GetDatasFromCookies();
        //    var existProduct = wishlist.FirstOrDefault(m => m.Id == id);
        //    if (existProduct == null)
        //    {
        //        wishlist.Add(new WishListDto { Id = id });
        //    }

        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));

        //    // DB-yə əlavə et
        //    var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        var wishlistFromDb = _context.WishLists
        //            .Include(w => w.WishListProducts)
        //            .FirstOrDefault(w => w.AppUserId == userId);

        //        if (wishlistFromDb == null)
        //        {
        //            wishlistFromDb = new Wishlist
        //            {
        //                AppUserId = userId,
        //                WishListProducts = new List<WishListProduct>()
        //            };

        //            _context.WishLists.Add(wishlistFromDb);
        //        }

        //        if (!wishlistFromDb.WishListProducts.Any(wp => wp.ProductId == id))
        //        {
        //            wishlistFromDb.WishListProducts.Add(new WishListProduct
        //            {
        //                ProductId = id
        //            });

        //            _context.SaveChanges();
        //        }
        //    }

        //    return wishlist.Count;
        //}

        //public void DeleteItem(int id)
        //{
        //    List<WishListDto> wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);

        //    WishListDto wishlistItem = wishlist.FirstOrDefault(m => m.Id == id);

        //    wishlist.Remove(wishlistItem);

        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));
        //}

        //public async Task<List<WishListProduct>> GetAllByWishlistIdAsync(int? wishlistId)
        //{
        //    return await _context.WishListProducts.Where(m => m.WishlistId == wishlistId).ToListAsync();
        //}

        //public async Task<WishListDto> GetByUserIdAsync(string userId)
        //{
        //    var wishList = await _context.WishLists
        //                                .Include(m => m.WishListProducts)
        //                                .ThenInclude(wp => wp.Product)
        //                                .FirstOrDefaultAsync(m => m.AppUserId == userId);

        //    return _mapper.Map<WishListDto>(wishList);
        //}

        //public int GetCount()
        //{
        //    List<WishListDto> wishlist;

        //    if (_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY] != null)
        //    {
        //        wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);
        //    }
        //    else
        //    {
        //        wishlist = new List<WishListDto>();

        //    }

        //    return wishlist.Count();
        //}

        //public List<WishListDto> GetDatasFromCookies()
        //{
        //    var data = _httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY];

        //    if (data is not null)
        //    {
        //        var wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(data);
        //        return wishlist;
        //    }
        //    else
        //    {
        //        return new List<WishListDto>();
        //    }
        //}

        //public async Task<List<WislistDetailDto>> GetWishlistDatasAsync()
        //{
        //    List<WishListDto> wishlist;

        //    if (_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY] != null)
        //    {
        //        wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);
        //    }
        //    else
        //    {
        //        wishlist = new List<WishListDto>();

        //    }

        //    List<WislistDetailDto> wishlistDetails = new();
        //    foreach (var item in wishlist)
        //    {
        //        ProductGetDto existProduct = await _productService.GetByIdAsync(item.Id);

        //        wishlistDetails.Add(new WislistDetailDto
        //        {
        //            Id = existProduct.Id,
        //            Name = existProduct.Name,
        //            Price = existProduct.CostPrice,
        //            Image = existProduct.MainFileImage
        //        });
        //    }
        //    return wishlistDetails;
        //}

        //public void SetDatasToCookies(List<WishListDto> wishlist, Product dbProduct, WishListDto existProduct)
        //{
        //    if (existProduct == null)
        //    {
        //        wishlist.Add(new WishListDto
        //        {
        //            Id = dbProduct.Id
        //        });
        //    }

        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));

        //}
        #endregion



    }
}
