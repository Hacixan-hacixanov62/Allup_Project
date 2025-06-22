using Allup_Service.Service.IService;
using Allup_Service.UI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;


        public ShopController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Shop/Detail/{id}")]   
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ShopDetailDto shopDetailDto = new ShopDetailDto
            {
                Product = product,
            };

            return View(shopDetailDto);
        }
    }
}
