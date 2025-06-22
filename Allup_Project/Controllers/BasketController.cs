using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public BasketController(IBasketService basketService, AppDbContext context, UserManager<AppUser> userManager )
        {
            _basketService = basketService;
            _context = context;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int count = 1)
        {
            await _basketService.AddToCartAsync(id, count);
            return Ok();
        }

        //public async Task<IActionResult> IncreaseToCart(int id, int count = 1)
        //{
        //    await _basketService.AddToCartAsync(id, count);

        //    var basket = await GetBasketAsync();

        //    return PartialView("_BasketModalPartial", basket);
        //}
        public async Task<IActionResult> DecreaseToCart(int id)
        {
            await _basketService.DecreaseToCartAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteBasket(int id)
        {
            await _basketService.DeleteBasket(id);
            return RedirectToAction(nameof(Index)); // Burada PartialView("Index") bele olmalidir amma exception verir
        }


    }
}
