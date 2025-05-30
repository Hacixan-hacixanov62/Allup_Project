using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public ProductController(ICategoryService categoryService, IProductService productService, AppDbContext context, IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index(int page =1,int take =6)
        {
            var products = _context.Products
                .Include(m=>m.Category)
                .Include(m=>m.ProductImages)
                .AsQueryable();
            
            PaginatedList<Product> paginatedList = PaginatedList<Product>.Create(products, page,take);

            return View(paginatedList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productCreateDto);
            }

            var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == productCreateDto.CategoryId);
            if (!isExistCategory)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(productCreateDto);
            }

            if (_context.Products.Any(x => x.Name == productCreateDto.Name))
            {
                ModelState.AddModelError("", "Product already exists");
                return View(productCreateDto);
            }

            await _productService.CreateAsync(productCreateDto);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id) 
        {
            if (id < 1) return NotFound();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var product = await _productService.DetailAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            ProductUpdateDto dto = _mapper.Map<ProductUpdateDto>(product);

            dto.ImagePaths = product.ProductImages.Where(x => !x.IsCover).Select(x => x.ImageUrl).ToList();
            dto.ImageIds = product.ProductImages.Where(x => !x.IsCover).Select(x => x.Id).ToList();
            dto.MainFileUrl = product.ProductImages.FirstOrDefault(x => x.IsCover)?.ImageUrl ?? "null";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateDto productUpdateDto)
        {
            if(!ModelState.IsValid)
            {
                return View(productUpdateDto);
            }

            var category = _context.Categories.FirstOrDefault(x => x.Id == productUpdateDto.CategoryId);
            if (category == null)
            {
                return View(productUpdateDto);
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();

            try
            {

                var existProduct = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);

                if (existProduct is null)
                {
                    return NotFound();
                }

                var isExist = await _context.Products.AnyAsync(x => x.Name == productUpdateDto.Name && x.Id != id);
                if (isExist)
                {
                    ModelState.AddModelError("Name", "Product already exists");
                    return View(productUpdateDto);
                }

                var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == productUpdateDto.CategoryId);
                if (!isExistCategory)
                {
                    ModelState.AddModelError("CategoryId", "Category is not found");
                    return View(productUpdateDto);
                }
              

                await _productService.EditAsync(id, productUpdateDto);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(productUpdateDto);
            }


        }

        public IActionResult DeleteProductImage(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var productImage = _context.ProductImages.Find(id);
            if (productImage is null)
            {
                return NotFound();
            }
            if (productImage.IsCover == true)
            {
                return BadRequest("");
            }
            _context.ProductImages.Remove(productImage);
            _context.SaveChanges();
            return RedirectToAction("edit", new { id = productImage.ProductId });
        }

        public IActionResult SetMainImage(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var productImage = _context.ProductImages.Find(id);
            if (productImage is null)
            {
                return NotFound();
            }
            var mainImage = _context.ProductImages.FirstOrDefault(b => b.IsCover == true && b.ProductId == productImage.ProductId);
            //mainImage.IsMain = null;
            productImage.IsCover = true;

            _context.SaveChanges();
            return RedirectToAction("edit", new { id = productImage.ProductId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction("Index");

        }

        [HttpGet("admin/product/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.DetailAsync(id);
            return View(product);
        }

    }
}
