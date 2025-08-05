using Allup_DataAccess.DAL;
using Allup_Service.UI.Vm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<IActionResult> Index()
        {
           var sliders =await _context.Sliders.ToListAsync();
            var banners =await _context.Banners.ToListAsync();
           var featuresBanners =await _context.FeaturesBanners.ToListAsync();
            var products = await _context.Products.Include(p=>p.ProductImages).Include(p=>p.Category).Take(8).ToListAsync();
           var reclamBanners = await _context.ReclamBanners.ToListAsync();
            var wishLists = await _context.WishlistItems.ToListAsync();
            HomeVM homeVM = new HomeVM
            {
                Slider = sliders,
                Banner = banners,
                FeaturesBanner = featuresBanners,
                Products = products,
                ReclamBanners = reclamBanners,
                WishListCount = wishLists.Count,
            };

            return View(homeVM);
        }
    }
}
