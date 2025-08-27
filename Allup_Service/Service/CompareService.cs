using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.CompareDtos;
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
    public class CompareService : ICompareService
    {
        private readonly ICompareRepository _compareRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public const string COMPARE_KEY = "Compare";
        public CompareService(IMapper mapper, AppDbContext context, IProductService productService, IHttpContextAccessor httpContextAccessor, ICompareRepository compareRepository)
        {
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _compareRepository = compareRepository;
        }



        public async Task<bool> AddToCompareAsync(int id, int count = 1)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found!");

            var user = _httpContextAccessor.HttpContext!.User;
            var isAuthenticated = user.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var existingItem = await _compareRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);
                if (existingItem != null)
                {
                    await _compareRepository.Delete(existingItem);
                    await _compareRepository.SaveChangesAsync();
                }
                else
                {
                    Compare wishListItem = new Compare
                    {
                        AppUserId = userId,
                        ProductId = id,
                        Count = count
                    };
                    await _compareRepository.CreateAsync(wishListItem);
                    await _compareRepository.SaveChangesAsync();
                }
            }
            else
            {
                var cookies = _httpContextAccessor.HttpContext.Response.Cookies;
                var requestCookies = _httpContextAccessor.HttpContext.Request.Cookies;

                List<CompareCookieDto> wishList = new List<CompareCookieDto>();

                string? cookieData = requestCookies[COMPARE_KEY];
                if (!string.IsNullOrEmpty(cookieData))
                {
                    wishList = JsonConvert.DeserializeObject<List<CompareCookieDto>>(cookieData) ?? new List<CompareCookieDto>();
                }

                var existingCookieItem = wishList.FirstOrDefault(x => x.ProductId == id);
                if (existingCookieItem != null)
                {
                    existingCookieItem.Count = count;
                }
                else
                {
                    wishList.Add(new CompareCookieDto
                    {
                        ProductId = id,
                        Count = count
                    });
                }

                string updatedCookie = JsonConvert.SerializeObject(wishList);
                cookies.Append(COMPARE_KEY, updatedCookie, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(14),
                    HttpOnly = false,
                    Secure = false
                });
            }

            return true;
        }

        public async Task<List<Compare>> GetCompareAsync()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = await _context.Compares.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == userId).ToListAsync();
                return basketItems;
            }

            var basktItms = _getCompare();
            foreach (var item in basktItms)
            {
                var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == item.ProductId);
                item.Product = product;


            }

            return basktItms;
        }

        public async Task<bool> RemoveFromCompareAsync(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found!");

            var user = _httpContextAccessor.HttpContext!.User;
            var isAuthenticated = user.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var wishlistItem = await _context.Compares
                    .FirstOrDefaultAsync(w => w.AppUserId == userId && w.ProductId == id);

                if (wishlistItem != null)
                {
                    _context.Compares.Remove(wishlistItem);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                // Guest user: Remove from cookie
                var cookie = _httpContextAccessor.HttpContext.Request.Cookies[COMPARE_KEY];
                if (cookie != null)
                {
                    var wishlist = JsonConvert.DeserializeObject<List<CompareCookieDto>>(cookie);
                    var item = wishlist.FirstOrDefault(x => x.ProductId == id);
                    if (item != null)
                    {
                        wishlist.Remove(item);
                        string updatedCookie = JsonConvert.SerializeObject(wishlist);
                        _httpContextAccessor.HttpContext.Response.Cookies.Append(COMPARE_KEY, updatedCookie);
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<CompareCardDto> CompareCardVM()
        {
            var result = new CompareCardDto
            {
                Product = new List<CompareItemCard>()
            };

            var user = _httpContextAccessor.HttpContext?.User;
            var isAuthenticated = user?.Identity != null && user.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                string userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var wishlistItems = _compareRepository
                    .GetFilter(x => x.AppUserId == userId, include: x => x.Include(p => p.Product))
                    .ToList();

                result.Product = wishlistItems.Select(x => new CompareItemCard
                {
                    ProductId = x.ProductId,
                    Product = x.Product,
                    Name = x.Product?.Name,
                    ImageUrl = x.Product?.MainFileImage,
                    Price = x.Product?.CostPrice ?? 0,
                    Count = x.Count,
                    Desc = x.Product?.Desc,
                   // Title = x.Product?.Title,
                    Color = x.Product?.ColorProducts.FirstOrDefault()?.Color.Name ?? "No Color",
                    InStock = x.Product?.StockCount ?? 0,
                   // Rating = x.Product != null ? (int)Math.Round((x.Product..Sum(r => r.Rate) / (x.Product.Ratings.Count == 0 ? 1 : x.Product.Ratings.Count))) : 0

                }).ToList();
            }
            else
            {
                var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[COMPARE_KEY];
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    var cookieItems = JsonConvert.DeserializeObject<List<CompareCookieDto>>(cookie);
                    if (cookieItems != null)
                    {
                        foreach (var item in cookieItems)
                        {
                            var product = await _productService.GetAsync(item.ProductId);
                            if (product != null)
                            {
                                result.Product.Add(new CompareItemCard
                                {
                                    ProductId = product.Id,
                                    Name = product.Name,
                                    ImageUrl = product.MainFileImage,
                                    Price = product.CostPrice,
                                    Desc = product.Desc,
                                    // Title = product.Title,
                                    Color = product.Colors.FirstOrDefault()?.Name ?? "No Color",
                                    InStock = product.StockCount,
                                    // Rating = product.Ratings.Count > 0 ? (int)Math.Round(product.Ratings.Sum(r => r.Rate) / product.Ratings.Count) : 0,
                                    Count = 1
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<int> CompareCount()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var wishlistItems = _compareRepository
                    .GetFilter(x => x.AppUserId == userId)
                    .ToList();

                return wishlistItems.Count;
            }
            else
            {
                var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[COMPARE_KEY];
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    var cookieItems = JsonConvert.DeserializeObject<List<WishListCookieItemDto>>(cookie);
                    return cookieItems?.Count ?? 0;
                }
            }

            return 0;
        }

        public async Task<int> GetIntAsync()
        {
            if (_checkAuthorized())
            {
                var userId = _getUserId();
                var count = await _context.Compares
                                          .Where(x => x.AppUserId == userId)
                                          .Select(x => x.ProductId)
                                          .Distinct()
                                          .CountAsync(); // Eyni məhsulu bir dəfə say
                return count;
            }

            var request = _httpContextAccessor.HttpContext.Request;
            if (request.Cookies[COMPARE_KEY] != null)
            {
                var basketItems = JsonConvert.DeserializeObject<List<Compare>>(request.Cookies[COMPARE_KEY]);
                var distinctCount = basketItems.Select(x => x.ProductId)
                                               .Distinct()
                                               .Count();
                return distinctCount;
            }

            return 0;
        }





        public List<Compare> _getCompare()
        {
            List<Compare> basketItems = new();
            if (_httpContextAccessor.HttpContext.Request.Cookies[COMPARE_KEY] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<Compare>>(_httpContextAccessor.HttpContext.Request.Cookies[COMPARE_KEY]) ?? new();
            }

            return basketItems;
        }



        private string _getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        private bool _checkAuthorized()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }


    }
}
