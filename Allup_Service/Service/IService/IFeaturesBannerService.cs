using Allup_Core.Entities;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.FeaturesBannerDtos;


namespace Allup_Service.Service.IService
{
    public interface IFeaturesBannerService
    {
        Task CreateAsync(FeaturesBannerCreateDto bannerCreateDto);
        Task DeleteAsync(int id);
        Task<FeaturesBannerDetailDto> DetailAsync(int id);
        Task<List<FeaturesBanner>> GetAllAsync();
        Task EditAsync(int id, FeaturesBannerUpdateDto bannerUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<FeaturesBanner> GetByIdAsync(int bannerId);
    }
}
