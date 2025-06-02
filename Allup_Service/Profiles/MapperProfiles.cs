using Allup_Core.Entities;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Dtos.TagDtos;
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

            //Brand Profiles
            CreateMap<Brand, BrandCreateDto>().ReverseMap();
            CreateMap<Brand, BrandUpdateDto>().ReverseMap();
            CreateMap<Brand, BrandDetailDto>().ReverseMap();

            //FeaturesBanner Profiles
            CreateMap<FeaturesBanner, FeaturesBannerCreateDto>().ReverseMap();
            CreateMap<FeaturesBanner, FeaturesBannerUpdateDto>().ReverseMap();
            CreateMap<FeaturesBanner, FeaturesBannerDetailDto>().ReverseMap();

            //Tag Profiles
            CreateMap<Tag, TagCreateDto>().ReverseMap();
            CreateMap<Tag, TagUpdateDto>().ReverseMap();
            CreateMap<Tag, TagGetDto>().ReverseMap();


        }
    }
}
