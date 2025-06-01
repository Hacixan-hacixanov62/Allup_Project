using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper mapper;
        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            this.mapper = mapper;
        }


        public async Task CreateAsync(BrandCreateDto brandCreateDto)
        { 
            Brand brand = mapper.Map<Brand>(brandCreateDto);
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = "admin"; // This should be replaced with actual user info from authentication context
            brand.UpdatedAt = DateTime.UtcNow;
            brand.UpdatedBy = "admin"; // This should be replaced with actual user info from authentication context

            await _brandRepository.CreateAsync(brand);
            await _brandRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetAsync(m=>m.Id ==id);
            if(brand == null)
            {
                throw new Exception("Brand not found");
            }

            await _brandRepository.Delete(brand);
            await _brandRepository.SaveChangesAsync();
        }

        public async Task<BrandDetailDto> DetailAsync(int id)
        {
            var brand =await _brandRepository.GetAll()
                .Where(m => m.Id == id)
                .Select(m => new BrandDetailDto
                {
                    Name = m.Name,
                    Products = m.Products
                }).FirstOrDefaultAsync();

            if (brand == null)
            {
                throw new Exception("Brand not found");
            }

            await _brandRepository.SaveChangesAsync();

            return  brand;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            var brand = await _brandRepository.GetAll().ToListAsync();
            return brand.Select(m => new Brand
            {
                Id = m.Id,
                Name = m.Name,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedAt = m.UpdatedAt,
                UpdatedBy = m.UpdatedBy
            }).ToList();
        }

        public  Task<Brand> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _brandRepository.IsExistAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(int id, BrandUpdateDto brandUpdateDto)
        {
            var brand =await _brandRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                throw new Exception("Brand not found");
            }

            brand.Name = brandUpdateDto.Name;
            brand.UpdatedAt = DateTime.UtcNow;
            brand.UpdatedBy = "admin";           // This should be replaced with actual user info from authentication context
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = "admin";

            _brandRepository.Update(brand);
        }
    }
}
