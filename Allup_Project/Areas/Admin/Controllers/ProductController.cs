 using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Superadmin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IColorService _colorService;
        private readonly ITagService _tagService;
        private readonly ISizeService _sizeService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public ProductController(ICategoryService categoryService, IProductService productService, AppDbContext context, IMapper mapper, ISizeService sizeService , ITagService tagService , IColorService colorService , IBrandService brandService )
        {
            _categoryService = categoryService;
            _productService = productService;
            _context = context;
            _mapper = mapper;
            _sizeService = sizeService;
            _tagService = tagService;
            _colorService = colorService;
            _brandService = brandService;
        }
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page =1,int take =6)
        {

            var product = await _productService.GetAllAsync();
            return View(product);
        }

        private async Task PopulateViewBags()
        {
        //    ViewBag.Brands = new SelectList(await _brandService.GetAllAsync(), "Id", "Name");
        //    ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
        //    ViewBag.Colors = new MultiSelectList(await _colorService.GetAllAsync(), "Id", "Name");
        //    ViewBag.Tags = new MultiSelectList(await _tagService.GetAllAsync(), "Id", "Name");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
        }
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto)
        {
            await PopulateViewBags();

            //if (!ModelState.IsValid)
            //    return View(productCreateDto);

            var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == productCreateDto.CategoryId);
            var isExistBrand = await _context.Brands.AnyAsync(x => x.Id == productCreateDto.BrandId);

            if (!isExistCategory)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(productCreateDto);
            }

            if (!isExistBrand)
            {
                ModelState.AddModelError("BrandId", "Brand not found");
                return View(productCreateDto);
            }

            var products = await _context.Products
                .Include(p => p.SizeProducts)
                .Include(p => p.ColorProducts)
                .Include(p => p.TagProducts)
                .ToListAsync();

            var existingProduct = products.FirstOrDefault(p =>
                p.Name == productCreateDto.Name
                && p.SizeProducts.Select(sp => sp.SizeId).OrderBy(id => id).SequenceEqual((productCreateDto.SizeIds ?? new List<int>()).OrderBy(id => id))
                && p.ColorProducts.Select(cp => cp.ColorId).OrderBy(id => id).SequenceEqual((productCreateDto.ColorIds ?? new List<int>()).OrderBy(id => id))
                && p.TagProducts.Select(tp => tp.TagId).OrderBy(id => id).SequenceEqual((productCreateDto.TagIds ?? new List<int>()).OrderBy(id => id))
            );

            if (existingProduct != null)
            {
                ModelState.AddModelError("", "This product with selected sizes, colors and tags already exists");
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

            var product = await _productService.DetailAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            await PopulateViewBags();


            ProductUpdateDto dto = _mapper.Map<ProductUpdateDto>(product);

            dto.ImagePaths = product.ProductImages.Where(x => !x.IsCover).Select(x => x.ImageUrl).ToList();
            dto.ImageIds = product.ProductImages.Where(x => !x.IsCover).Select(x => x.Id).ToList();
            dto.MainFileUrl = product.ProductImages.FirstOrDefault(x => x.IsCover)?.ImageUrl ?? "null";

            // Many-to-Many üçün ID-ləri map et
            dto.SizeIds = product.SizeProducts?.Select(sp => sp.SizeId).ToList()?? new List<int>();
            dto.ColorIds = product.ColorProducts?.Select(cp => cp.ColorId).ToList() ?? new List<int>();
            dto.TagIds = product.TagProducts?.Select(tp => tp.TagId).ToList() ?? new List<int>();

            return View(dto);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateDto productUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                return View(productUpdateDto);
            }

            var categoryExists = await _context.Categories.AnyAsync(x => x.Id == productUpdateDto.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "Category is not found");
                await PopulateViewBags();
                return View(productUpdateDto);
            }

            var nameExists = await _context.Products.AnyAsync(x => x.Name == productUpdateDto.Name && x.Id != id);
            if (nameExists)
            {
                ModelState.AddModelError("Name", "Product already exists");
                await PopulateViewBags();
                return View(productUpdateDto);
            }

            // Null yoxlaması many-to-many üçün
            productUpdateDto.SizeIds ??= new();
            productUpdateDto.ColorIds ??= new();
            productUpdateDto.TagIds ??= new();

            try
            {
                await _productService.EditAsync(id, productUpdateDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateViewBags();
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

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/product/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.DetailAsync(id);
            return View(product);
        }

    }
}
