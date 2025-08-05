using Allup_Core.Entities;
using Allup_Service.Dtos.WisListDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Allup_Project.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IWishListService _wishListService;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistController(IHttpContextAccessor httpContextAccessor, IProductService productService, IWishListService wishListService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _wishListService = wishListService;
        }

        public async Task<IActionResult> Index()
        {
           // var wishlistDetails = await _wishListService.WishListCardVM();
           var  wishlistDetails = await _wishListService.GetWishListAsync();
            return View(wishlistDetails);

        }

        [HttpPost]
        public async Task<IActionResult> AddWishlist(int id)
        {
           await _wishListService.AddToWishListAsync(id);
            return Json(new { success = true });

        }


        [HttpPost]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var success = await _wishListService.RemoveFromWishListAsync(id);
            return Json(new { success });
        }


        public async Task<IActionResult> GetWishlistCount()
        {
            List<WishListDto> wishlist = new();
            if (Request.Cookies[WishListService.WishList_KEY] != null)
            {
                wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(Request.Cookies[WishListService.WishList_KEY]);
            }
            var count = wishlist.Count;
            return Json(new { success = true, count });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseToCart(int id)
        {
            await _wishListService.AddToWishListAsync(id);

            var basket = await _wishListService.GetWishListAsync();

            // PartialView qaytar və ya JSON qaytar
            // Məsələn, sadə JSON qaytaraq:

            int totalCount = basket.Sum(x => x.Count);
            decimal totalPrice = basket.Sum(x => x.Count * (x.Product.CostPrice - (x.Product.SalePrice * x.Product.DiscountPercent / 100)));

            return Json(new { success = true, count = totalCount, total = totalPrice });
        }


        [HttpPost]
        public async Task<IActionResult> DecreaseToCart(int id)
        {
            await _wishListService.DecreaseToCartAsync(id);

            var basket = await _wishListService.GetWishListAsync();

            int totalCount = basket.Sum(x => x.Count);
            decimal totalPrice = basket.Sum(x => x.Count * (x.Product.CostPrice - (x.Product.SalePrice * x.Product.DiscountPercent / 100)));

            return Json(new { success = true, count = totalCount, total = totalPrice });
        }


        [HttpGet]
        [Route("wishlist/getdecimalsubtotalasync")]
        public async Task<IActionResult> GetDecimalSubTotalAsync()
        {
            decimal total = 0;

            var basketItems = await _wishListService.GetWishListAsync();

            foreach (var item in basketItems)
            {
                if (item.Product != null)
                {
                    if (item.Product.DiscountPercent > 0)
                    {
                        total += (item.Product.CostPrice - (item.Product.SalePrice * item.Product.DiscountPercent / 100)) * item.Count;
                    }
                    else
                    {
                        total += item.Product.SalePrice * item.Count;

                    }
                }
            }

            return Json(new
            {
                total = total
            });
        }


        public async Task<IActionResult> getCountWishList()
        {
            var temCount = await _wishListService.GetIntAsync();

            return Json(new
            {
                count = temCount
            });
        }

    }  
}
