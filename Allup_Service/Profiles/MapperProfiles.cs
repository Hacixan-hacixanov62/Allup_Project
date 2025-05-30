using Allup_Core.Entities;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ProductDtos;
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

            //Product Profiles
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();
            CreateMap<Product, ProductGetDto>()
 .ForMember(dest => dest.MainFileUrl, opt => opt.MapFrom(src =>
     src.ProductImages.FirstOrDefault(img => img.IsCover).ImageUrl))
 .ReverseMap();

            //Banner Profiles
            CreateMap<Banner, BannerCreateDto>().ReverseMap();
            CreateMap<Banner, BannerUpdateDto>().ReverseMap();
            CreateMap<Banner, BannerGetDto>().ReverseMap();


        }
    }
}
