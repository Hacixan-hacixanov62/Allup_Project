using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Dtos.CartDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace Allup_Project.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly IBasketService _basketService;
        public PaymentController(AppDbContext context, UserManager<AppUser> userManager, IEmailService emailService, IBasketService basketService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Checkout()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var basketDetails = await _basketService.GetBasketAsync(); // burdan gəlməlidir

            if (basketDetails == null || !basketDetails.Any())
            {
                return RedirectToAction("Index", "Basket");
            }


            var domain = "http://localhost:5112/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Payment/Success",
                CancelUrl = domain + "Payment/failed",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in basketDetails)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.CostPrice * 100), // price * 100 cent
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public async Task<IActionResult> Success()
        {
            var basketCookie = Request.Cookies[BasketService.BASKET_KEY];

            if (string.IsNullOrEmpty(basketCookie))
            {
                return RedirectToAction("Index", "Home");
            }

            var basketItems = JsonConvert.DeserializeObject<List<CartGetDto>>(basketCookie);

            if (basketItems == null || !basketItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Silinməsi
            Response.Cookies.Delete(BasketService.BASKET_KEY);

            try
            {
                await SendSuccessEmailAsync(basketItems);
            }
            catch { }

            return RedirectToAction("Index", "Home", new { payment = "success" });
        }

        private async Task SendSuccessEmailAsync(List<CartGetDto> basketItems)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                Console.WriteLine("User tapılmadı - login olmayıb");
                return;
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                Console.WriteLine("User email-i yoxdur");
                return;
            }

            Console.WriteLine($"Email göndəriləcək: {user.Email}");

            var subject = "Ödəniş uğurla tamamlandı";

            string body = $@"
              <!DOCTYPE html>
              <html lang='az'>
              <head>
              <meta charset='UTF-8'>
              <meta name='viewport' content='width=device-width, initial-scale=1.0'>
              <title>Ödəniş Təsdiqi</title>
              <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .email-container {{
                max-width: 600px;
                margin: 20px auto;
                background-color: #fff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            }}
            .email-header, .email-footer {{
                background-color: #4CAF50;
                color: white;
                text-align: center;
                padding: 10px;
            }}
            .email-body {{
                padding: 20px;
                color: #333;
            }}
            .email-body h2 {{
                color: #333;
            }}
            .product-details {{
                margin: 20px 0;
            }}
            .product-details table {{
                width: 100%;
                border-collapse: collapse;
            }}
            .product-details th, .product-details td {{
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }}
            .product-details th {{
                background-color: #f2f2f2;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-header'>
                <h1>Ödəniş Təsdiqi</h1>
            </div>
            <div class='email-body'>
                <h2>Salam {user.UserName},</h2>
                <p>Sizin ödənişiniz uğurla tamamlandı. Alınan məhsullar aşağıdakı kimidir:</p>
                <div class='product-details'>
                    <table>
                        <thead>
                            <tr>
                                <th>Məhsul</th>
                                <th>Sayı</th>
                                <th>Cəmi</th>
                            </tr>
                        </thead>
                        <tbody>";
            var basketDetails = await _basketService.GetBasketAsync(); // burdan gəlməlidir
            foreach (var item in basketDetails)
            {
                body += $@"
                            <tr>
                                <td>{item.Product.Name}</td>
                                <td>{item.Count}</td>
                                <td>${item:F2}</td>
                            </tr>";
            }

            body += @"
                        </tbody>
                    </table>
                </div>
                <p>Bizə güvəndiyiniz üçün təşəkkür edirik!</p>
            </div>
            <div class='email-footer'>
                <p>&copy; 2025 Final MozArt. Bütün hüquqlar qorunur.</p>
            </div>
        </div>
    </body>
    </html>";

          await  _emailService.SendEmailAsync(new() { Body = body, Subject = subject, ToEmail = user.Email });
        }
    }
}
