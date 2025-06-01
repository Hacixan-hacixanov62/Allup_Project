using Allup_Core.Entities;
using Allup_Service.Dtos.BrandDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface IBrandService
    {
        Task CreateAsync(BrandCreateDto brandCreateDto);
        Task UpdateAsync(int id, BrandUpdateDto brandUpdateDto);
        Task DeleteAsync(int id);
        Task<Brand> GetByIdAsync(int id);
        Task<List<Brand>> GetAllAsync();
        Task<BrandDetailDto> DetailAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
