using Allup_Core.Entities;
using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Dtos.ReclamBannerDtos;

namespace Allup_Service.Service.IService
{
    public interface IReclamBannerService
    {
        Task CreateAsync(ReclamBannerCreateDto bannerCreateDto);
        Task DeleteAsync(int id);
        Task<ReclamBannerDetailDto> DetailAsync(int id);
        Task<List<ReclamBanner>> GetAllAsync();
        Task EditAsync(int id, ReclamBannerUpdateDto bannerUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<ReclamBanner> GetByIdAsync(int bannerId);
    }
}
