using Allup_Core.Entities;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface ISizeService
    {
        Task CreateAsync(SizeCreateDto sizeCreateDto);
        Task DeleteAsync(int id);
        Task<SizeGetDto> DetailAsync(int id);
        Task<List<Size>> GetAllAsync();
        Task EditAsync(int id, SizeUpdateDto sizeUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<SizeGetDto> GetByIdAsync(int sizeId);
    }
}
