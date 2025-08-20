using Allup_Core.Entities;
using Allup_DataAccess.Helpers;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.AboutDtos;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;
        private readonly IAboutRepository _aboutRepository;
        public AboutController(IAboutService aboutService, IAboutRepository aboutRepository)
        {
            _aboutService = aboutService;
            _aboutRepository = aboutRepository;
        }
        
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page = 1, int take = 4)
        {
            var abouts =await _aboutService.GetAllAsync();
            //PaginatedList<About> paginateList = PaginatedList<About>.Create(abouts, page, take);
            return View(abouts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutCreateDto aboutCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(aboutCreateDto);
            //}
            await _aboutService.CreateAsync(aboutCreateDto);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var about = _aboutService.DetailAsync(id.Value).Result;
            if (about == null)
            {
                return NotFound();
            }
            var aboutUpdateDto = new AboutUpdateDto
            {
                Title = about.Title,
                Title1 = about.Title1,
                Title2 = about.Title2,
                Title3 = about.Title3,
                Desc = about.Desc,
                Desc1 = about.Desc1,
                Desc2 = about.Desc2,
                Desc3 = about.Desc3,
                ImageUrl = about.ImageUrl
            };
            return View(aboutUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutUpdateDto aboutUpdateDto)
        {
            try
            {
                await _aboutService.EditAsync(id, aboutUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(aboutUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _aboutService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/about/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var about = await _aboutService.DetailAsync(id);
            if (about == null)
            {
                return NotFound();
            }
            return View(about);
        }
    }
}
