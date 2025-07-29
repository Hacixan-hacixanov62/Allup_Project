using Allup_Core.Entities;
using Allup_Service.Dtos.WisListDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            List<WishListDto> wishlist = new();

            if (Request.Cookies[WishListService.WishList_KEY] != null)
            {
                wishlist = JsonConvert.DeserializeObject<List<WishListDto>>(Request.Cookies[WishListService.WishList_KEY]);
            }

            var ids = wishlist.Select(x => x.Id).ToList();

            var products = await _productService.GetProductsByIdsAsync(ids);

            var wishlistDetails = products.Select(p => new WislistDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.CostPrice,
                Image = p.MainFileImage
            }).ToList();

            return View(wishlistDetails);

        }

        [HttpPost]
        public IActionResult AddWishlist(int id)
        {
            var product = _productService.GetByIdAsync(id).Result;
            if (product == null)
                return Json(new { success = false });

            var count = _wishListService.AddWishlist(id, product);

            return Json(new { success = true, count = count });
        }

        [HttpPost]
        public IActionResult DeleteWishlist(int id)
        {
            _wishListService.DeleteItem(id);

            return Json(new { success = true });
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


    }
}
