using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
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
