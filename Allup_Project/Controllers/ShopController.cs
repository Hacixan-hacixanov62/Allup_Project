using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Extensions;
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
        private readonly ICommentService _commentService;
        private readonly IFeaturesBannerService _featuresBannerService;
        public ShopController(IProductService productService, ICategoryService categoryService, IMapper mapper, ISizeService sizeService, IColorService colorService, ICommentService commentService, IFeaturesBannerService featuresBannerService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _mapper = mapper;
            _sizeService = sizeService;
            _colorService = colorService;
            _commentService = commentService;
            _featuresBannerService = featuresBannerService;
        }

        public async Task<IActionResult> Index(string sortOrder, List<int> sizeIds, List<int> colorIds, int? minPrice =null, int? maxPrice =null, int page = 1, int pageSize = 5)
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var sizes = await _sizeService.GetAllAsync();
            var colors = await _colorService.GetAllAsync();
            var banner = await _featuresBannerService.GetAllAsync();

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

            // Pagination
            int totalCount = products.Count; 
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productDtos = _mapper.Map<List<ProductGetDto>>(products);
            var categoryDtos = _mapper.Map<List<CategoryGetDto>>(categories);
            var sizeDtos = _mapper.Map<List<SizeGetDto>>(sizes);
            var colorDtos = _mapper.Map<List<ColorGetDto>>(colors);    

            var minDbPrice = products.Min(p => p.SalePrice); //CostPrice yerine SalePrice istifadə edirik, çünki bu qiymət endirimli qiymətdir
            var maxDbPrice = products.Max(p => p.SalePrice); 

            ViewBag.MinDbPrice = minDbPrice;
            ViewBag.MaxDbPrice = maxDbPrice;

            var shopDto = new ShopDto
            {
                Products = productDtos,
                Categories = categoryDtos,
                Sizes = sizeDtos,
                Colors = colorDtos,
                SelectedSize = sizeIds,
                SelectedColor = colorIds,
                SelectedMinPrice = minPrice,
                SelectedMaxPrice = maxPrice,
                Index = page,
                Size = pageSize,
                Count = totalCount,
                Pages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages,
                FeaturesBanners = banner,
            };

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
            var comments = await _commentService.GetComment(id);

            ShopDetailDto shopDetailDto = new ShopDetailDto
            {
                Product = product,
                Comments = comments,
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




        [HttpPost]
        public async Task<IActionResult> CreateComment([Bind(Prefix = "CommentCreateDto")] CommentCreateDto dto)
        {
            var result = await _commentService.CreateAsync(dto, ModelState);

            string returnUrl = Request.GetReturnUrl();

            var comment = await _commentService.GetComment(dto.ProductId);
            return PartialView("_CommentListPartial", comment);
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> ReplyComment(CommentReplyDto dto)
        {

            var result = await _commentService.CreateReplyAsync(dto, ModelState);

            string returnUrl = Request.GetReturnUrl();

            return Redirect(returnUrl);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentService.DeleteAsync(id);

            string returnUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }



    }
}
