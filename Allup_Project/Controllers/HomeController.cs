using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        public HomeController(AppDbContext context, IProductService productService, IBlogService blogService)
        {
            _context = context;
            _productService = productService;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
           var sliders =await _context.Sliders.ToListAsync();
            var banners =await _context.Banners.ToListAsync();
           var featuresBanners =await _context.FeaturesBanners.ToListAsync();
            var products = await _context.Products.Include(p=>p.ProductImages).Include(p=>p.Category).Take(8).ToListAsync();
           var reclamBanners = await _context.ReclamBanners.ToListAsync();
            var wishLists = await _context.WishlistItems.ToListAsync();
            var blogs = await _blogService.GetAllAsync();  
            var comments = await _context.Comments.OrderByDescending(x => x.Rating).Include(x => x.AppUser).Take(3).ToListAsync();
            HomeVM homeVM = new HomeVM
            {
                Slider = sliders,
                Banner = banners,
                FeaturesBanner = featuresBanners,
                Products = products,
                ReclamBanners = reclamBanners,
                WishListCount = wishLists.Count,
                Blogs = blogs,
                Comments = comments
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
    }
}
