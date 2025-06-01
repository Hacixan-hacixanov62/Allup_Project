using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly AppDbContext _context;
        public BrandController(IBrandService brandService, AppDbContext context)
        {
            _brandService = brandService;
            _context = context;
        }

        public IActionResult Index(int page = 1, int take = 4)
        {
            var brands = _context.Brands.Include(m => m.Products).OrderByDescending(m => m.CreatedAt).AsQueryable();
            PaginatedList<Brand> paginatedList = PaginatedList<Brand>.Create(brands, take, page);
            return View(paginatedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandCreateDto brandCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(brandCreateDto);
            }
            await _brandService.CreateAsync(brandCreateDto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _brandService.DetailAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            var brandUpdateDto = new BrandUpdateDto
            {
                Name = brand.Name,
            };
            return View(brandUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandUpdateDto brandUpdateDto)
        {
            try
            {
                await _brandService.UpdateAsync(id, brandUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(brandUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _brandService.DeleteAsync(id);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("admin/brand/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = await _brandService.DetailAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        public async Task<IActionResult> IsExist(int id)
        {
            var isExist = await _brandService.IsExistAsync(id);
            return Json(isExist);
        }

    }
}
