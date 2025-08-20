using Allup_Core.Entities;
using Allup_Service.Dtos.AboutDtos;
using Allup_Service.Dtos.BannerDtos;

namespace Allup_Service.Service.IService
{
    public interface IAboutService
    {
        Task CreateAsync(AboutCreateDto aboutCreateDto );
        Task DeleteAsync(int id);
        Task<AboutGetDto> DetailAsync(int id);
        Task<List<About>> GetAllAsync();
        Task EditAsync(int id, AboutUpdateDto aboutUpdateDto );

        Task<bool> IsExistAsync(int id);
        Task<AboutGetDto> GetByIdAsync(int aboutId);
    }
}
