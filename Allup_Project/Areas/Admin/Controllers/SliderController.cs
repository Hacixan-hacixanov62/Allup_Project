using Allup_Core.Entities;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]

    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index()
        {
            var sliders = await _sliderService.GetAllAsync();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateDto sliderCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(sliderCreateDto);
            //}

            await _sliderService.CreateAsync(sliderCreateDto);
                
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var slider = await _sliderService.DetailAsync(id.Value);
            if (slider == null)
                return NotFound();

            var sliderUpdateDto = new SliderUpdateDto
            {
                Title = slider.Title,
                Desc = slider.Desc,
                ImageUrl = slider.Image 
            };

            return View(sliderUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderUpdateDto sliderUpdateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(sliderUpdateDto);
            //}

            try
            {
                await _sliderService.EditAsync(id, sliderUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(sliderUpdateDto);
            }
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {

            await _sliderService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/slider/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var slider = await _sliderService.DetailAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);

        }

    }
}
