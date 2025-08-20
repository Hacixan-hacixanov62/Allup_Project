using Allup_Service.Service.IService;
using Allup_Service.UI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;
        private readonly IFeaturesBannerService _featuresBannerService;
        private readonly IBrandService _brandService;
        public AboutController(IAboutService aboutService, IFeaturesBannerService featuresBannerService, IBrandService brandService)
        {
            _aboutService = aboutService;
            _featuresBannerService = featuresBannerService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllAsync();
            var featuresBanners = await _featuresBannerService.GetAllAsync();
            var abouts = await _aboutService.GetAllAsync();

            var aboutVM = new AboutDto
            {
                Brands = brands,
                FeaturesBanners = featuresBanners,
                Abouts = abouts
            };

            return View(aboutVM);
        }
    }
}
