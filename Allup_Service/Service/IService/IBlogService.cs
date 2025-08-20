
using Allup_Core.Entities;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.BlogDtos;
using Allup_Service.UI.Vm;

namespace Allup_Service.Service.IService
{
    public interface IBlogService
    {
        Task CreateAsync(BlogCreateDto blogCreateDto);
        Task DeleteAsync(int id);
        Task<Blog> DetailAsync(int id);
        Task<List<Blog>> GetAllAsync();
        Task EditAsync(int id, BlogUpdateDto blogUpdateDto);
        Task<bool> IsExistAsync(int id);
        Task<PaginationResponse<Blog>> GetPaginateAsync(int page, int take, string search = null);
    }
}
