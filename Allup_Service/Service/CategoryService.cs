using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Allup_Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IWebHostEnvironment _env;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IWebHostEnvironment env, ICloudinaryService cloudinaryService)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _env = env;
            _cloudinaryService = cloudinaryService;
        }


        public async Task CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            if(categoryCreateDto.ImageFile == null)
            {
                throw new ArgumentNullException("ImageFile", "This area is required!");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Categories");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = categoryCreateDto.ImageFile.FileName;

            if (fileName.Length > 64)
            {
                fileName = fileName.Substring(fileName.Length - 64, 64);
            }

            fileName = Guid.NewGuid().ToString() + fileName;

            string path = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await categoryCreateDto.ImageFile.CopyToAsync(fileStream);
            }

            Category category = _mapper.Map<Category>(categoryCreateDto);   

            category.ImageUrl = fileName;
            category.CreatedBy = "admin";
            category.UpdatedBy = "admin";
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;


            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = _categoryRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (category == null)
            {
                throw new Exception("Slider not found");
            }
            
            string path = Path.Combine(_env.WebRootPath, "Uploads/Categories", category.ImageUrl);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<Category> DetailAsync(int id)
        {
           var category = _categoryRepository.GetAll()
                .Where(s => s.Id == id)
                .Select(s => new Category
                {
                    Id = s.Id,
                    Name = s.Name,
                    ImageUrl = s.ImageUrl,                

                }).FirstOrDefault();

            if (category == null)
            {
                throw new Exception("Category not found");
            }
            
            return category;
        }



        public async Task EditAsync(int id, CategoryUpdateDto categoryUpdateDto)
        {
            var category = _categoryRepository.GetAll().FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Categories");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (categoryUpdateDto.ImageFile != null)
            {
                // Köhnə şəkili sil
                string oldImagePath = Path.Combine(folderPath, category.ImageUrl);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Yeni şəkilin adını formalaşdır
                string fileName = categoryUpdateDto.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                fileName = Guid.NewGuid().ToString() + fileName;

                string newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await categoryUpdateDto.ImageFile.CopyToAsync(stream);
                }

                category.ImageUrl = fileName;
            }

           // Category updatedCategory = _mapper.Map<Category>(categoryUpdateDto);
                category.Name = categoryUpdateDto.Name;
                category.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAll().ToListAsync();
            return categories.Select(s => new Category
            {
                Id = s.Id,               
                Name = s.Name,
                ImageUrl = s.ImageUrl,

                // CreatedDate = s.CreatedDate.ToString("yyyy-MM-dd")
            }).ToList();
        }

        public Task<Category> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetSingleAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
