using Allup_Core.Entities;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.SliderDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface ICategoryService
    {
         Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes);
         Task<Category> GetSingleAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes);
         Task<Category> GetByIdAsync(int id);
        Task CreateAsync(CategoryCreateDto categoryCreateDto);
        Task DeleteAsync(int id);
        Task<Category> DetailAsync(int id);
        Task<List<Category>> GetAllAsync();
        Task EditAsync(int id, CategoryUpdateDto sliderUpdateDto);
    }
}
