using Allup_Core.Entities;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface ITagService
    {
        Task CreateAsync(TagCreateDto tagCreateDto);
        Task DeleteAsync(int id);
        Task<TagGetDto> DetailAsync(int id);
        Task<List<Tag>> GetAllAsync();
        Task EditAsync(int id, TagUpdateDto tagUpdateDto);

        Task<bool> IsExistAsync(int id);
        Task<TagGetDto> GetByIdAsync(int tagId);
    }
}
