using Allup_Core.Entities;
using Allup_Service.Dtos.AuthorDtos;

namespace Allup_Service.Service.IService
{
    public interface IAuthorService
    {
        Task CreateAsync(AuthorCreateDto chefCreateDto);
        Task DeleteAsync(int id);
        Task<Author> DetailAsync(int id);
        Task<List<Author>> GetAllAsync();
        Task EditAsync(int id, AuthorUpdateDto chefUpdateDto);
    }
}
