using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]

    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;
        private readonly AppDbContext _context;
        public SizeController(ISizeService sizeService, AppDbContext context)
        {
            _sizeService = sizeService;
            _context = context;
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index()
        {
            var size = await _sizeService.GetAllAsync();
            //var size = _context.Sizes.Include(m => m.Products).ThenInclude(m=>m.SizeProducts).OrderByDescending(m => m.CreatedAt).AsQueryable();
            // PaginatedList<Size> paginatedList = PaginatedList<Size>.Create(size,page,take);
            return View(size);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SizeCreateDto sizeCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(sizeCreateDto);
            //}
            await _sizeService.CreateAsync(sizeCreateDto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var size = await _sizeService.DetailAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            var brandUpdateDto = new SizeUpdateDto
            {
                Name = size.Name,
            };
            return View(brandUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SizeUpdateDto sizeUpdateDto)
        {
            try
            {
                await _sizeService.EditAsync(id, sizeUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(sizeUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _sizeService.DeleteAsync(id);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/Size/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var size = await _sizeService.DetailAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        public async Task<IActionResult> IsExist(int id)
        {
            var isExist = await _sizeService.IsExistAsync(id);
            return Json(isExist);
        }


    }
}
