using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeaturesBannerController : Controller
    {
        private readonly IFeaturesBannerService _featuresBannerService;

        public FeaturesBannerController(IFeaturesBannerService featuresBannerService)
        {
            _featuresBannerService = featuresBannerService;
        }


        public async Task<IActionResult> Index(int page = 1,int take =4)
        {
            var banner = await _featuresBannerService.GetAllAsync();
            return View(banner);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeaturesBannerCreateDto featuresBannerCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(featuresBannerCreateDto);
            //}

           await _featuresBannerService.CreateAsync(featuresBannerCreateDto);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var banner = await _featuresBannerService.DetailAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            var bannrUpdatedto = new FeaturesBannerUpdateDto
            {
                Title = banner.Title,
                Desc = banner.Desc,
                ImageUrl = banner.ImageUrl
            };


            return View(bannrUpdatedto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,FeaturesBannerUpdateDto featuresBannerUpdateDto)
        {

            try
            {
                await _featuresBannerService.EditAsync(id, featuresBannerUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(featuresBannerUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _featuresBannerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("admin/FeaturesBanner/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var banner = await _featuresBannerService.DetailAsync(id);
            if(banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }
    }
}
