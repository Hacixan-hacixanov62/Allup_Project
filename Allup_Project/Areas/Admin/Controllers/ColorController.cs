using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.ColorDtos;
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

    public class ColorController : Controller
    {
        private readonly IColorService _colorService;
        private readonly AppDbContext _context;
        public ColorController(IColorService colorService, AppDbContext context)
        {
            _colorService = colorService;
            _context = context;
        }
        public IActionResult Index(int page = 1, int take = 4)
        {
            var tags = _context.Colors.Include(m => m.Products).ThenInclude(m => m.ColorProducts).OrderByDescending(m => m.CreatedAt).AsQueryable();
            PaginatedList<Color> paginatedList = PaginatedList<Color>.Create(tags, take, page);
            return View(paginatedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ColorCreateDto tagCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(tagCreateDto);
            }
            await _colorService.CreateAsync(tagCreateDto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _colorService.DetailAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            var brandUpdateDto = new ColorUpdateDto
            {
                Name = tag.Name,
            };
            return View(brandUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ColorUpdateDto tagUpdateDto)
        {
            try
            {
                await _colorService.EditAsync(id, tagUpdateDto);
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
            await _colorService.DeleteAsync(id);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("admin/Color/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _colorService.DetailAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        public async Task<IActionResult> IsExist(int id)
        {
            var isExist = await _colorService.IsExistAsync(id);
            return Json(isExist);
        }
    }
}
