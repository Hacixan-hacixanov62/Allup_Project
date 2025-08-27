using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service.Generic;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class ProductService:CrudService<Product, ProductCreateDto, ProductUpdateDto, ProductGetDto>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment env, IMapper mapper , ICloudinaryService cloudinaryService , AppDbContext context , IHttpContextAccessor httpContextAccessor ) : base(productRepository, mapper)
        {
            _productRepository = productRepository;
            _env = env;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateAsync(ProductCreateDto productCreateDto)
        {
            var product = _mapper.Map<Product>(productCreateDto);
            product.ProductImages = new List<ProductImage>();

            // --- Main image ---
            var mainFileName = await _cloudinaryService.FileCreateAsync(productCreateDto.MainFile);
            var mainImage = CreateProductImage(mainFileName, true, product);
            product.ProductImages.Add(mainImage);

            // --- Additional images ---
            if (productCreateDto.AdditionalFiles is not null)
            {
                foreach (var file in productCreateDto.AdditionalFiles)
                {
                    var fileName = await _cloudinaryService.FileCreateAsync(file);
                    var additionalImage = CreateProductImage(fileName, false, product);
                    product.ProductImages.Add(additionalImage);
                }
            }

            // --- SizeProducts ---
            //product.SizeProducts = new List<SizeProduct>();
            //if (productCreateDto.SizeIds is not null)
            //{
            //    foreach (var sizeId in productCreateDto.SizeIds.Distinct())
            //    {
            //        product.SizeProducts.Add(new SizeProduct
            //        {
            //            SizeId = sizeId
            //        });
            //    }
            //}

            product.SizeProducts = productCreateDto.SizeIds.Select(sizeId => new SizeProduct   // .Distinct() bu methodan istifade ede bilersen tekraralari silmek ucun
            {
                SizeId = sizeId,
                Product = product
            }).ToList();

            // --- ColorProducts ---
            //product.ColorProducts = new List<ColorProduct>();
            //if (productCreateDto.ColorIds is not null)
            //{
            //    foreach (var colorId in productCreateDto.ColorIds.Distinct())
            //    {
            //        product.ColorProducts.Add(new ColorProduct
            //        {
            //            ColorId = colorId
            //        });
            //    }
            //}

            product.ColorProducts = productCreateDto.ColorIds.Select(colorId => new ColorProduct // .Distinct() bu methodan istifade ede bilersen tekraralari silmek ucun
            {
                ColorId = colorId,
                Product =product
            }).ToList();

            // --- TagProducts ---

            //product.TagProducts = new List<TagProduct>();
            //if (productCreateDto.TagIds is not null)
            //{
            //    foreach (var tagId in productCreateDto.TagIds.Distinct())
            //    {
            //        product.TagProducts.Add(new TagProduct
            //        {
            //            TagId = tagId
            //        });
            //    }
            //}

            product.TagProducts = productCreateDto.TagIds.Select(tagId => new TagProduct // .Distinct() bu methodan istifade ede bilersen tekraralari silmek ucun
            {
                TagId = tagId,
                Product = product
            }).ToList();

            // --- Metadata ---
            var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedBy =username; // TODO: Replace with real user
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = username;
            product.SalePrice = productCreateDto.SalePrice ;
            product.DiscountPercent = productCreateDto.DiscountPercent;
            product.CostPrice = productCreateDto.CostPrice;

            // --- Save to DB ---
            await _productRepository.CreateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        private ProductImage CreateProductImage(string imageUrl, bool isCover, Product product)
        {
            return new ProductImage
            {
                ImageUrl = imageUrl,
                IsCover = isCover,
                Product = product
            };
        }

        public async Task DeleteAsync(int id)
        {
            var product = _productRepository.GetAll().FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new Exception( "Product not found");
            }

            await _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<Product> DetailAsync(int id)
        {
            var product = await _productRepository.GetAll()
                     .Include(m => m.Category)
                .Include(m => m.ProductImages)
                .Include(m => m.Brands)
                .Include(m => m.SizeProducts)
                .ThenInclude(m => m.Size)
                .Include(m => m.ColorProducts)
                .ThenInclude(m => m.Color)
                .Include(m => m.TagProducts)
                .ThenInclude(m => m.Tag)
                      .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task EditAsync(int id, ProductUpdateDto productUpdateDto)
        { 
            var product = _productRepository.GetAll().Include(p => p.SizeProducts)
                                                         .Include(p => p.ColorProducts)
                                                         .Include(p => p.TagProducts)
                                                         .Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var existproduct = _productRepository.GetAll()
                         .Include(m=>m.Category)
                         .Include(m=>m.Brands)
                         .Include(m=>m.ProductImages)
                         .FirstOrDefault(p => p.Id == id);

            if(existproduct == null)
            {
                throw new Exception("Product not found");
            }

            if (productUpdateDto.CategoryId != existproduct.CategoryId || productUpdateDto.BrandId != existproduct.BrandId)
            {
                if (!_context.Categories.Any(g => g.Id == productUpdateDto.CategoryId))
                {
                    throw new Exception("Category not found");
                }
                if (!_context.Brands.Any(g => g.Id == productUpdateDto.BrandId))
                {
                    throw new Exception("Brand not found");
                }
            }


            var ExistedImages = existproduct.ProductImages.Where(x => !x.IsCover).Select(x => x.Id).ToList();
            if (productUpdateDto.ProductImageIds is not null)
            {
                ExistedImages = ExistedImages.Except(productUpdateDto.ProductImageIds).ToList();

            }
            if (ExistedImages.Count > 0)
            {
                foreach (var imageId in ExistedImages)
                {
                    var deletedImage = existproduct.ProductImages.FirstOrDefault(x => x.Id == imageId);
                    if (deletedImage is not null)
                    {

                        existproduct.ProductImages.Remove(deletedImage);

                        deletedImage.ImageUrl.DeleteFile(_env.WebRootPath, "assets/images/product");
                    }

                }
            }


            foreach (var file in productUpdateDto.AdditionalFiles)
            {
                var filename = await _cloudinaryService.FileCreateAsync(file);
                var additionalProductImages = new ProductImage() { IsCover = false, Product = existproduct, ImageUrl = filename };
                existproduct.ProductImages.Add(additionalProductImages);

            }


            if (productUpdateDto.MainFile is not null)
            {
                var existMainImage = existproduct.ProductImages.FirstOrDefault(x => x.IsCover);
                //remove exist image
                if (existMainImage is not null)
                {
                    existproduct.ProductImages.Remove(existMainImage);

                }



                var filename = await _cloudinaryService.FileCreateAsync(productUpdateDto.MainFile);
                ProductImage image = new ProductImage() { IsCover = true, Product = existproduct, ImageUrl = filename };
                existproduct.ProductImages.Add(image);

            }

            // Clear and update SizeProducts
            existproduct.SizeProducts.Clear();
            foreach (var sizeId in productUpdateDto.SizeIds.Distinct())
            {
                existproduct.SizeProducts.Add(new SizeProduct
                {
                    SizeId = sizeId,
                    ProductId = existproduct.Id
                });
            }

            // Clear and update ColorProducts
            existproduct.ColorProducts.Clear();
            foreach (var colorId in productUpdateDto.ColorIds.Distinct())
            {
                existproduct.ColorProducts.Add(new ColorProduct
                {
                    ColorId = colorId,
                    ProductId = existproduct.Id
                });
            }

            // Clear and update TagProducts
            existproduct.TagProducts.Clear();
            foreach (var tagId in productUpdateDto.TagIds.Distinct())
            {
                existproduct.TagProducts.Add(new TagProduct
                {
                    TagId = tagId,
                    ProductId = existproduct.Id
                });
            }


            existproduct = _mapper.Map(productUpdateDto, existproduct);

            _productRepository.Update(existproduct);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products =await _productRepository.GetAll(include : x=>x
                                                      .Include(m => m.Category)
                                                     .Include(m=>m.ProductImages)
                                                     .Include(m => m.Brands)
                                                     .Include(m => m.TagProducts).ThenInclude(tp => tp.Tag)
                                                     .Include(m => m.ColorProducts).ThenInclude(cp => cp.Color)
                                                    .Include(m => m.SizeProducts).ThenInclude(sp => sp.Size)
                                                  )
                                                  .ToListAsync();
            return products;
        }

        public async Task<ProductGetDto> GetByIdAsync(int productId)
        {
            var product = await _context.Products.Include(p => p.ProductImages)
                                                      .Include(m => m.Category)
                                                     .Include(m => m.Brands)
                                                     .Include(m => m.TagProducts).ThenInclude(tp => tp.Tag)
                                                     .Include(m => m.ColorProducts).ThenInclude(cp => cp.Color)
                                                    .Include(m => m.SizeProducts).ThenInclude(sp => sp.Size)
                                                    .FirstOrDefaultAsync(p => p.Id == productId);   

            if (product == null) return null;

            var dto = _mapper.Map<ProductGetDto>(product);

            dto.Tags = product.TagProducts.Select(tp => new TagGetDto
            {
                Id = tp.Tag.Id,
                Name = tp.Tag.Name
            }).ToList();

            dto.Colors = product.ColorProducts.Select(cp => new ColorGetDto
            {
                Id = cp.Color.Id,
                Name = cp.Color.Name
            }).ToList();

            dto.Sizes = product.SizeProducts.Select(sp => new SizeGetDto
            {
                Id = sp.Size.Id,
                Name = sp.Size.Name
            }).ToList();

            

            return dto;

        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _productRepository.IsExistAsync(p => p.Id == id);
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();

                products = products.Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    p.Category.Name.ToLower().Contains(query));
            }

            return await products.ToListAsync();
        }

        public Task<ICollection<ProductGetDto>> SortAsync(string sortKey)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<ProductGetDto>> FilterAsync(string? categoryName, string? brandName, string? tagName)
        {
            IQueryable<Product> query = _context.Products
               .Include(p => p.Category)
               .Include(p => p.Brands)
               .Include(p => p.TagProducts).ThenInclude(pt => pt.Tag)
               .Include(p => p.SizeProducts).ThenInclude(sp => sp.Size)
               .Include(p => p.ColorProducts).ThenInclude(cp => cp.Color)
               .Include(p => p.ProductImages)
               .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                categoryName = categoryName.Trim();
                // Case-insensitive comparison əlavə edin
                query = query.Where(p => p.Category != null &&
                           EF.Functions.Like(p.Category.Name, categoryName));

            }

            if (!string.IsNullOrWhiteSpace(brandName))
            {
                brandName = brandName.Trim();
                // Case-insensitive comparison əlavə edin
                query = query.Where(p => p.Brands != null &&
                                   p.Brands.Name.ToLower() == brandName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                tagName = tagName.Trim();
                // Case-insensitive comparison əlavə edin
                query = query.Where(p => p.TagProducts.Any(pt => pt.Tag != null &&
                                   pt.Tag.Name.ToLower() == tagName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                tagName = tagName.Trim();
                // Case-insensitive comparison əlavə edin
                query = query.Where(p => p.SizeProducts.Any(pt => pt.Size != null &&
                                   pt.Size.Name.ToLower() == tagName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(tagName))
            {
                tagName = tagName.Trim();
                // Case-insensitive comparison əlavə edin
                query = query.Where(p => p.ColorProducts.Any(pt => pt.Color != null &&
                                   pt.Color.Name.ToLower() == tagName.ToLower()));
            }

            var filteredProducts = await query.ToListAsync();
            return _mapper.Map<ICollection<ProductGetDto>>(filteredProducts);
        }

        public async Task<List<ProductGetDto>> GetProductAsync(int skip, int take)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brands)
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .Select(p => new ProductGetDto
                    {
                        Id = p.Id,
                        Name = p.Name ?? "",
                        Desc =p.Desc,
                        CostPrice = p.CostPrice,
                        SalePrice = p.SalePrice,
                        DiscountPercent = p.DiscountPercent
                        //Brand =p.Brands !=null ? p.Brands.Name : "" ,
                        
                    })
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetProductsAsync Error: {ex.Message}");
                return new List<ProductGetDto>();
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            try
            {
                return await _context.Products.CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTotalProductCountAsync Error: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<ProductGetDto>> FilterByPriceAsync(decimal min, decimal max)
        {
            var products = await _context.Products
               .Where(p => p.CostPrice >= min && p.CostPrice <= max)
                 .Where(p => p.SalePrice >= min && p.SalePrice <= max)
               .Include(p => p.Brands)
               .Include(p => p.Category)
                .Include(p => p.TagProducts).ThenInclude(tp => tp.Tag)
                .Include(p => p.SizeProducts).ThenInclude(sp => sp.Size)
                .Include(p => p.ColorProducts).ThenInclude(cp => cp.Color)
               .Include(p => p.ProductImages)
               .AsNoTracking()
               .ToListAsync();

            return _mapper.Map<List<ProductGetDto>>(products);
        }


        public async Task<List<ProductGetDto>> GetProductsByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<ProductGetDto>();

            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ProductGetDto
                {
                    Id = p.Id,
                    Name = p.Name,  
                    CostPrice = p.CostPrice,
                    SalePrice = p.SalePrice,
                    DiscountPercent = p.DiscountPercent,
                    Desc = p.Desc,
                    MainFileImage = p.ProductImages.FirstOrDefault(m => m.IsCover).ImageUrl

                })
                .ToListAsync();
        }
         
        public async Task<ICollection<ProductGetDto>> SearchAsync(string searchText, int page, int take)
        {
            var products = _context.Products.Where(p=>p.Name.Contains(searchText) || p.Desc.Contains(searchText))
                .Include(p => p.Category)
                .Include(p => p.Brands)
                .Include(p => p.TagProducts).ThenInclude(tp => tp.Tag)
                .Include(p => p.SizeProducts).ThenInclude(sp => sp.Size)
                .Include(p => p.ColorProducts).ThenInclude(cp => cp.Color)
                .Include(p => p.ProductImages)
                .AsNoTracking()
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();

            return   _mapper.Map<ICollection<ProductGetDto>>(products);
        }

    }
}
