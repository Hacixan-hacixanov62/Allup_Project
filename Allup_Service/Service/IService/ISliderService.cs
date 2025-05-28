using Allup_Core.Entities;
using Allup_Service.Dtos.SliderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface ISliderService
    {
        Task CreateAsync(SliderCreateDto sliderCreateDto);  
        Task DeleteAsync(int id);   
        Task<Slider> DetailAsync(int id);
        Task<List<Slider>> GetAllAsync();
        Task EditAsync(int id, SliderUpdateDto sliderUpdateDto);
    }
}
