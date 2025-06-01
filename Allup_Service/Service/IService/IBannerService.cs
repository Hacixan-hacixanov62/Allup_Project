using Allup_Core.Entities;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface IBannerService
    {
        Task CreateAsync(BannerCreateDto bannerCreateDto);
        Task DeleteAsync(int id);
        Task<BannerGetDto> DetailAsync(int id);
        Task<List<Banner>> GetAllAsync();
        Task EditAsync(int id, BannerUpdateDto bannerUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<BannerGetDto> GetByIdAsync(int bannerId);
    }
}
