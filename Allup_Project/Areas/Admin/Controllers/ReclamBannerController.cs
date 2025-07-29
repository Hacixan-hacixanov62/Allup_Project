using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Dtos.ReclamBannerDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    public class ReclamBannerController : Controller
    {
        private readonly IReclamBannerService _reclamBannerService;

        public ReclamBannerController(IReclamBannerService reclamBannerService)
        {
            _reclamBannerService = reclamBannerService;
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page = 1, int take = 4)
        {
            var reclamBanners = await _reclamBannerService.GetAllAsync();
            return View(reclamBanners);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReclamBannerCreateDto reclamBannerCreateDto)
        {
            await _reclamBannerService.CreateAsync(reclamBannerCreateDto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var reclamBanner =await _reclamBannerService.DetailAsync(id);
            if (reclamBanner == null)
            {
                return NotFound();
            }
          
            var reclamBannerUpdateDto = new ReclamBannerUpdateDto
            {
                ImageUrl = reclamBanner.ImageUrl,
                RedirectUrl = reclamBanner.RedirectUrl,
            };

            return View(reclamBannerUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReclamBannerUpdateDto reclamBannerUpdateDto)
        {
            try
            {
                await _reclamBannerService.EditAsync(id, reclamBannerUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(reclamBannerUpdateDto);
            }
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/ReclamBanner/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var reclamBanner = await _reclamBannerService.DetailAsync(id);
            if (reclamBanner == null)
            {
                return NotFound();
            }
            return View(reclamBanner);
        }

        public async Task<IActionResult> Delete(int id)
        {
            
            await _reclamBannerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
