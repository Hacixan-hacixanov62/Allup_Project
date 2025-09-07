using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Dtos.CommanDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        private readonly ICurrencyService _currencyService;
        public HomeController(AppDbContext context, IProductService productService, IBlogService blogService, ICurrencyService currencyService)
        {
            _context = context;
            _productService = productService;
            _blogService = blogService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            string selectedCurrency = Request.Cookies["currency"] ?? "USD";

            var sliders =await _context.Sliders.ToListAsync();
            var banners =await _context.Banners.ToListAsync();
           var featuresBanners =await _context.FeaturesBanners.ToListAsync();
            var products = await _context.Products.Include(p=>p.ProductImages).Include(p=>p.Category).Take(8).ToListAsync();

            foreach (var product in products)
            {
                product.SalePrice =  _currencyService.ConvertCurrencyAsync(product.SalePrice,"AZN",selectedCurrency);
            }

            var reclamBanners = await _context.ReclamBanners.ToListAsync();
            var wishLists = await _context.WishlistItems.ToListAsync();
            var blogs = await _blogService.GetAllAsync();  
            var comments = await _context.Comments.OrderByDescending(x => x.Rating).Include(x => x.AppUser).Take(3).ToListAsync();
            var compares = await _context.Compares.ToListAsync();
            HomeVM homeVM = new HomeVM
            {
                Slider = sliders,
                Banner = banners,
                FeaturesBanner = featuresBanners,
                Products = products,
                ReclamBanners = reclamBanners,
                WishListCount = wishLists.Count,
                Blogs = blogs,
                Comments = comments,
                CompareCount = compares.Count,
                SelectedCurrency = selectedCurrency
            };

            return View(homeVM);
        }

        public async Task<IActionResult> Search(string query)
        {
            var products =await _productService.SearchProductsAsync(query);

            var productDtos = products.Select(m => new Product
            {
                Id = m.Id,
                Name = m.Name,
                Desc = m.Desc
            }).ToList();

            var vm = new HomeVM
            {
                Products = productDtos
            };

            return View("Index", vm);
        }

        public IActionResult SetCurrency(string currency)
        {
            // Cookie yaz
            Response.Cookies.Append("currency", currency, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30) // 30 gün yadda saxla
            });

            // İstifadəçini əvvəlki səhifəyə qaytar
            string returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error(string? json)
        {
            if (!string.IsNullOrEmpty(json))
            {

                string decodedJson = Uri.UnescapeDataString(json);

                var dto = JsonConvert.DeserializeObject<ErrorDto>(decodedJson);
                return View(dto);
            }

            return View(new ErrorDto
            {
                StatusCode = 500,
                Message = "Gözlənilməyən xəta baş verdi."
            });
        }
    }
}
