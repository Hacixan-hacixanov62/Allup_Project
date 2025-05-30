using Allup_Core.Entities;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.SliderDtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Profiles
{
    public class MapperProfiles:Profile
    {
        public MapperProfiles()
        {
            // Slider Profiles
            CreateMap<Slider,SliderCreateDto>().ReverseMap();
            CreateMap<Slider, SliderUpdateDto>().ReverseMap();

            //Category Profiles 
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryDetailDto>().ReverseMap();

        }
    }
}
