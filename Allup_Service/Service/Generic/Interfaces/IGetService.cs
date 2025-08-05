using Allup_Service.Abstractions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.Generic
{
    public interface IGetService<TGetDto>
    where TGetDto : IDto
    {
        Task<TGetDto> GetAsync(int id);
        Task<List<TGetDto>> GetAllAsync();
    }
}
