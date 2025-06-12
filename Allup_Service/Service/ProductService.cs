using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service
{
    public class ProductService:IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _context;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment env, IMapper mapper = null, ICloudinaryService cloudinaryService = null, AppDbContext context = null)
        {
            _productRepository = productRepository;
            _env = env;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _context = context;
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
            product.SizeProducts = new List<SizeProduct>();
            if (productCreateDto.SizeIds is not null)
            {
                foreach (var sizeId in productCreateDto.SizeIds.Distinct())
                {
                    product.SizeProducts.Add(new SizeProduct
                    {
                        SizeId = sizeId
                    });
                }
            }

            // --- ColorProducts ---
            product.ColorProducts = new List<ColorProduct>();
            if (productCreateDto.ColorIds is not null)
            {
                foreach (var colorId in productCreateDto.ColorIds.Distinct())
                {
                    product.ColorProducts.Add(new ColorProduct
                    {
                        ColorId = colorId
                    });
                }
            }

            // --- TagProducts ---
            product.TagProducts = new List<TagProduct>();
            if (productCreateDto.TagIds is not null)
            {
                foreach (var tagId in productCreateDto.TagIds.Distinct())
                {
                    product.TagProducts.Add(new TagProduct
                    {
                        TagId = tagId
                    });
                }
            }

            // --- Metadata ---
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedBy = "admin"; // TODO: Replace with real user
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = "admin";

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
                      .Include(m=>m.Category)
                      .Include(m => m.ProductImages)
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
            var product = _productRepository.GetAll().FirstOrDefault(p => p.Id == id);
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
            var products =await _productRepository.GetAll(include : x=>x.Include(m=>m.ProductImages)).ToListAsync();
            return products;
        }

        public async Task<ProductGetDto> GetByIdAsync(int productId)
        {
            var product = await _productRepository.GetAsync(x => x.Id == productId,
                                                      x => x.Include(p => p.ProductImages));
            if (product == null) return null;

            var dto = _mapper.Map<ProductGetDto>(product);
            return dto;
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _productRepository.IsExistAsync(p => p.Id == id);
        }
    }
}
