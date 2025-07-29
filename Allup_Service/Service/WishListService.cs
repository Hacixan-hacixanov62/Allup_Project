using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.WisListDtos;
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
        private readonly IWishListRepository _wishListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public const string WishList_KEY = "wishlist";

        public WishListService(IWishListRepository wishListRepository, AppDbContext context, IProductService productService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _wishListRepository = wishListRepository;
            _context = context;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public int AddWishlist(int id, ProductGetDto product)
        {
            // Cookies hissəsi (var)
            List<WishListDto> wishlist = GetDatasFromCookies();
            var existProduct = wishlist.FirstOrDefault(m => m.Id == id);
            if (existProduct == null)
            {
                wishlist.Add(new WishListDto { Id = id });
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));

            // DB-yə əlavə et
            var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var wishlistFromDb = _context.WishLists
                    .Include(w => w.WishListProducts)
                    .FirstOrDefault(w => w.AppUserId == userId);

                if (wishlistFromDb == null)
                {
                    wishlistFromDb = new Wishlist
                    {
                        AppUserId = userId,
                        WishListProducts = new List<WishListProduct>()
                    };

                    _context.WishLists.Add(wishlistFromDb);
                }

                if (!wishlistFromDb.WishListProducts.Any(wp => wp.ProductId == id))
                {
                    wishlistFromDb.WishListProducts.Add(new WishListProduct
                    {
                        ProductId = id
                    });

                    _context.SaveChanges();
                }
            }

            return wishlist.Count;
        }
        
        public void DeleteItem(int id)
        {
            List<WishListDto> wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);

            WishListDto wishlistItem = wishlist.FirstOrDefault(m => m.Id == id);

            wishlist.Remove(wishlistItem);

            _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));
        }

        public async Task<List<WishListProduct>> GetAllByWishlistIdAsync(int? wishlistId)
        {
            return await _context.WishListProducts.Where(m => m.WishlistId == wishlistId).ToListAsync();
        }

        public async Task<WishListDto> GetByUserIdAsync(string userId)
        {
            var wishList = await _context.WishLists
                                        .Include(m => m.WishListProducts)
                                        .ThenInclude(wp => wp.Product) 
                                        .FirstOrDefaultAsync(m => m.AppUserId == userId);

            return _mapper.Map<WishListDto>(wishList);
        }

        public int GetCount()
        {
            List<WishListDto> wishlist;

            if (_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY] != null)
            {
                wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);
            }
            else
            {
                wishlist = new List<WishListDto>();

            }

            return wishlist.Count();
        }

        public List<WishListDto> GetDatasFromCookies()
        {
            var data = _httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY];

            if (data is not null)
            {
                var wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(data);
                return wishlist;
            }
            else
            {
                return new List<WishListDto>();
            }
        }

        public async Task<List<WislistDetailDto>> GetWishlistDatasAsync()
        {
            List<WishListDto> wishlist;

            if (_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY] != null)
            {
                wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(_httpContextAccessor.HttpContext.Request.Cookies[WishList_KEY]);
            }
            else
            {
                wishlist = new List<WishListDto>();

            }

            List<WislistDetailDto> wishlistDetails = new();
            foreach (var item in wishlist)
            {
                ProductGetDto existProduct = await _productService.GetByIdAsync(item.Id);

                wishlistDetails.Add(new WislistDetailDto
                {
                    Id = existProduct.Id,
                    Name = existProduct.Name,
                    Price = existProduct.CostPrice,
                    Image = existProduct.MainFileImage
                });
            }
            return wishlistDetails;
        }

        public void SetDatasToCookies(List<WishListDto> wishlist, Product dbProduct, WishListDto existProduct)
        {
            if (existProduct == null)
            {
                wishlist.Add(new WishListDto
                {
                    Id = dbProduct.Id
                });
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append(WishList_KEY, JsonConvert.SerializeObject(wishlist));

        }
    }
}
