using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Service.IService;
using Allup_Service.UI.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ISizeService _sizeService;
        private readonly IColorService _colorService;

        public ShopController(IProductService productService, ICategoryService categoryService, IMapper mapper, ISizeService sizeService, IColorService colorService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _mapper = mapper;
            _sizeService = sizeService;
            _colorService = colorService;
        }

        public async Task<IActionResult> Index(string sortOrder, List<int> sizeIds, List<int> colorIds, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var sizes = await _sizeService.GetAllAsync();
            var colors = await _colorService.GetAllAsync();

            if (sizeIds != null && sizeIds.Any())
            {
                products = products
                    .Where(p => p.SizeProducts.Any(ps => sizeIds.Contains(ps.SizeId)))
                    .ToList();
            }

            if (colorIds != null && colorIds.Any())
            {
                products = products
                    .Where(p => p.ColorProducts.Any(pc => colorIds.Contains(pc.ColorId)))
                    .ToList();
            }

            // MinPrice və MaxPrice range filter
            if (minPrice != null && maxPrice != null)
            {
                products = products
                    .Where(p => p.SalePrice >= minPrice && p.SalePrice <= maxPrice)
                    .ToList();
            }
            else if (minPrice != null)
            {
                products = products
                    .Where(p => p.SalePrice >= minPrice)
                    .ToList();
            }
            else if (maxPrice != null)
            {
                products = products
                    .Where(p => p.SalePrice <= maxPrice)
                    .ToList();
            }


            sortOrder ??= "Default";

            products = sortOrder switch
            {
                "A_to_Z" => products.OrderBy(p => p.Name).ToList(),
                "Z_to_A" => products.OrderByDescending(p => p.Name).ToList(),
                "PriceLowToHigh" => products.OrderBy(p => p.SalePrice).ToList(),
                "PriceHighToLow" => products.OrderByDescending(p => p.SalePrice).ToList(),
                _ => products.OrderBy(p => p.Name).ToList(),
            };

            ViewData["SelectedSort"] = sortOrder;

            var productDtos = _mapper.Map<List<ProductGetDto>>(products);
            var categoryDtos = _mapper.Map<List<CategoryGetDto>>(categories);
            var sizeDtos = _mapper.Map<List<SizeGetDto>>(sizes);
            var colorDtos = _mapper.Map<List<ColorGetDto>>(colors);
            var shopDto = new ShopDto
            {
                Products = productDtos,
                Categories = categoryDtos,
                Sizes = sizeDtos,
                Colors = colorDtos,
                SelectedSize = sizeIds,
                SelectedColor = colorIds,
                SelectedMinPrice = minPrice,
                SelectedMaxPrice = maxPrice
            };


            var minDbPrice = products.Min(p => p.SalePrice);
            var maxDbPrice = products.Max(p => p.SalePrice);

            ViewBag.MinDbPrice = minDbPrice;
            ViewBag.MaxDbPrice = maxDbPrice;


            // Əgər AJAX requestdirsə, sadəcə məhsulları json qaytar , Shopdaki produclari refressiz isletmek uzundur
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var result = productDtos.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.CostPrice,
                    discount = p.DiscountPercent,
                    productImgs = p.ProductImages.Select(i => new { url = i.ImageUrl, isMain = i.IsCover })
                });

                return Json(new
                {
                    products = result,
                    count = result.Count()
                });
            }


            return View(shopDto);
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


        [HttpGet]
        public async Task<IActionResult> GetSortedProducts(string? sortKey)
        {
            List<ProductGetDto> products;

            if (string.IsNullOrWhiteSpace(sortKey) || sortKey == "default")
                products =  _productService.GetAllAsync().Result.Select(p => new ProductGetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CostPrice = p.CostPrice,
                    SalePrice = p.SalePrice,
                    DiscountPercent = p.DiscountPercent,
                    StockCount = p.StockCount,
                    Desc = p.Desc,
                    ProductCode = p.ProductCode
                }).ToList();
            else
                products = _productService.SortAsync(sortKey).Result.ToList();

            var result = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                DiscountPercent = p.DiscountPercent,
                StockCount = p.StockCount,
                Desc = p.Desc,
                ProductCode = p.ProductCode
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredProducts(string? categoryName, string? brandName, string? tagName)
        {
            var products = await _productService.FilterAsync(categoryName, brandName, tagName);

            var result = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                DiscountPercent = p.DiscountPercent,
                StockCount = p.StockCount,
                Desc = p.Desc,
                ProductCode = p.ProductCode
            });

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> FilterByPrice(decimal min, decimal max)
        {
            try
            {
                var products = await _productService.FilterByPriceAsync(min, max);

                return Json(products); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
