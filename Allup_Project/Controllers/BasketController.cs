using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Dtos.OrderDtos;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

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


         
        public async Task<IActionResult> Index()
        {
            var featuresBanners = await _context.FeaturesBanners.ToListAsync();
            var cartItems = await GetBasketAsync();
            BasketVM basketVM = new BasketVM
            {
                FeaturesBanners = featuresBanners,
                CartItems = cartItems
            };
            return View(basketVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int count = 1)
        {
            await _basketService.AddToCartAsync(id, count);
            return Ok();
        }

        public async Task<IActionResult> IncreaseToCart(int id, int count = 1)
        {
            await _basketService.AddToCartAsync(id, count);

            var basket = await GetBasketAsync();

            return PartialView("_BasketModalPartial", basket);
        }
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
        public async Task<IActionResult> GetCartSection()
        {
            var cart = _basketService.GetCartAsync();
            return PartialView("_cartSectionPartial", cart);
        }



        [HttpPost]
        public async Task<IActionResult> RemoveToCart(int id)
        {
            await _basketService.RemoveToCartAsync(id);
            return Ok();
        }

        [HttpGet]
        [Route("basket/getdecimalsubtotalasync")]
        public async Task<IActionResult> GetDecimalSubTotalAsync()
        {
            decimal total = 0;

            var basketItems = await GetBasketAsync();

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
        private async Task<List<CartItem>> GetBasketAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = await _context.CartItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == userId).ToListAsync();
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
        private List<CartItem> _getBasket()
        {
            List<CartItem> basketItems = new();
            if (Request.Cookies["AllupCart"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies["AllupCart"]) ?? new();
            }

            return basketItems;
        }

        public IActionResult GetBasket()
        {
            var basket = _basketService.GetUserBasketItem();
            return PartialView("_BasketPartial", basket.Items);
        }

        public async Task<IActionResult> getCountBasket()
        {
            var temCount = await _basketService.GetIntAsync();

            return Json(new
            {
                count = temCount
            });
        }


        // CheckOut Page in BasketController ===================================================

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            var basketItems = await GetBasketAsync();

            decimal total = basketItems.Sum(x => x.Product.CostPrice * x.Count);

            var vm = new CheckoutDto
            {
                BasketItems = basketItems,
                Total = total,
                Order = new OrderCreateDto()
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutDto vm)
        {
            var dto = vm.Order;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return BadRequest();

            var basketItems = await GetBasketAsync();
            decimal total = basketItems.Sum(b => b.Product.CostPrice * b.Count);



            //// Stripe-in kodd hissesi
            ////========================================================================================

            //var optionCust = new CustomerCreateOptions
            //{
            //    Email = dto.stripeEmail,
            //    Name = user.FullName,
            //    Phone = "994 051 516"
            //};
            //var serviceCust = new CustomerService();
            //Customer customer = serviceCust.Create(optionCust);

            //total = total * 100;
            //var optionsCharge = new ChargeCreateOptions  // Odenisin Melumatlari saxlanilir
            //{

            //    Amount = (long)total, //Dollari cente cevirir
            //    Currency = "USD",
            //    Description = "Dannys Restourant Order",
            //    Source = dto.stripeToken,
            //    ReceiptEmail = "hajikhanih@code.edu.az"


            //};
            //var serviceCharge = new ChargeService();
            //Charge charge = serviceCharge.Create(optionsCharge);

            ////===========================================================================
            Order order = new()
            {
                AppUser = user,
                Status = false,
                OrderItems = new List<OrderItem>(),
                PhoneNumber = dto.PhoneNumber,
                City = dto.City,
                Apartment = dto.Apartment,
                Street = dto.Street,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                CompanyName = dto.CompanyName,
                Town = dto.Town,
                Country = dto.Country
            };

            foreach (var bItem in basketItems)
            {
                OrderItem orderItem = new()
                {
                    Order = order,
                    Product = bItem.Product,
                    Count = bItem.Count,
                    TotalPrice = bItem.Product.CostPrice
                };
                order.OrderItems.Add(orderItem);
                _context.CartItems.Remove(bItem);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { orderId = order.Id });
        }

    }
}
