using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]

    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly AppDbContext _context;
        public TagController(ITagService tagService, AppDbContext context)
        {
            _tagService = tagService;
            _context = context;
        }

        public IActionResult Index(int page = 1, int take = 4)
        {
            var tags = _context.Tags.Include(m=>m.Products).ThenInclude(m=>m.TagProducts).OrderByDescending(m => m.CreatedAt).AsQueryable();
            PaginatedList<Tag> paginatedList = PaginatedList<Tag>.Create(tags, take, page);
            return View(paginatedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagCreateDto tagCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(tagCreateDto);
            }
            await _tagService.CreateAsync(tagCreateDto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _tagService.DetailAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            var brandUpdateDto = new TagUpdateDto
            {
                Name = tag.Name,
            };
            return View(brandUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TagUpdateDto tagUpdateDto)
        {
            try
            {
                await _tagService.EditAsync(id, tagUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(tagUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("admin/Tag/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _tagService.DetailAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        public async Task<IActionResult> IsExist(int id)
        {
            var isExist = await _tagService.IsExistAsync(id);
            return Json(isExist);
        }

    }
}
