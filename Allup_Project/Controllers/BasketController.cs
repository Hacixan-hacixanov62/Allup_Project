using Allup_Core.Entities;
using Allup_Core.Enums;
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
            var cartItems = await _basketService.GetBasketAsync();
            var cartItemsFto = await _basketService.GetCartAsync();
            BasketVM basketVM = new BasketVM
            {
                FeaturesBanners = featuresBanners,
                CartItems = cartItems,
                Cart = cartItemsFto,
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

            var basket = await _basketService.GetBasketAsync();

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

            var basketItems = await _basketService.GetBasketAsync();

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
            var basketItems = await _basketService.GetBasketAsync();
             
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

            var basketItems = await _basketService.GetBasketAsync();
            decimal total = basketItems.Sum(b => b.Product.CostPrice * b.Count);

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

        #region Stripe

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
        #endregion

        [Authorize(Roles = "Member")]
        public IActionResult Cancel(int orderId)
        {
            var order = _context.Orders
                .Where(m => m.AppUserId == _userManager.GetUserId(User))
                .FirstOrDefault(m => m.Id == orderId);
            order.OrderStatus = OrderStatus.Cancelled;
            _context.SaveChanges();
            return RedirectToAction("Profile", "Account", new { tab = "orders" });
        }

        [Authorize(Roles = "Member")]
        public IActionResult GetOrderItems(int orderId)
        {
            var order = _context.Orders
                .Where(m => m.AppUserId == _userManager.GetUserId(User))
                .Include(m => m.OrderItems)
                .FirstOrDefault(m => m.Id == orderId);
            return PartialView("_OrderItemsPartial", order);
        }

    }
}
