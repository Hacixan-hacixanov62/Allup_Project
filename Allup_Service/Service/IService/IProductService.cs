using Allup_Core.Entities;
using Allup_Service.Dtos.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface IProductService
    {
        Task CreateAsync(ProductCreateDto productCreateDto);
        Task DeleteAsync(int id);
        Task<Product> DetailAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task EditAsync(int id, ProductUpdateDto productUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<ProductGetDto> GetByIdAsync(int productId);

    }
}
