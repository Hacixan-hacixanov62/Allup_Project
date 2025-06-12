using Allup_Core.Entities;
using Allup_Service.Dtos.ColorDtos;


namespace Allup_Service.Service.IService
{
    public interface IColorService
    {
        Task CreateAsync(ColorCreateDto sizeCreateDto);
        Task DeleteAsync(int id);
        Task<ColorGetDto> DetailAsync(int id);
        Task<List<Color>> GetAllAsync();
        Task EditAsync(int id, ColorUpdateDto sizeUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<ColorGetDto> GetByIdAsync(int sizeId);
    }
}
