using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.SliderDtos;
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

    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;

        public CategoryController(ICategoryService categoryService, AppDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page = 1, int take = 2)
        {
            var categories = _context.Categories.Include(c => c.Products).OrderByDescending(m => m.CreatedAt).AsQueryable();
            PaginatedList<Category> paginatedList = PaginatedList<Category>.Create(categories, take, page);
            return View(paginatedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto categoryCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(categoryCreateDto);
            //}
            await _categoryService.CreateAsync(categoryCreateDto);
                 
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if(id ==null)
            {
                 return NotFound();
            }

            var category = await _categoryService.DetailAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryUpdateDto = new CategoryUpdateDto
            {
                Name = category.Name,
                ImageUrl = category.ImageUrl
            };


            return View(categoryUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryUpdateDto categoryUpdateDto)
        {
            
            try
            {
                await _categoryService.EditAsync(id, categoryUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(categoryUpdateDto);
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {

            await _categoryService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/category/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _categoryService.DetailAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

    }
}
