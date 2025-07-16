using Allup_Core.Entities;
using Allup_Service.Dtos.AuthDtos;
using Allup_Service.Dtos.AuthorDtos;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.BlogDtos;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.CartDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Dtos.OrderDtos;
using Allup_Service.Dtos.OrderItemDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.ReclamBannerDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Dtos.TagDtos;
using AutoMapper;


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
            CreateMap<Category , CategoryGetDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ReverseMap();

            //Product Profiles
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();
            CreateMap<Product, ProductGetDto>()
 .ForMember(dest => dest.MainFileUrl, opt => opt.MapFrom(src =>
     src.ProductImages.FirstOrDefault(img => img.IsCover).ImageUrl))
 .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brands))
 .ReverseMap();
           

            //Banner Profiles
            CreateMap<Banner, BannerCreateDto>().ReverseMap();
            CreateMap<Banner, BannerUpdateDto>().ReverseMap();
            CreateMap<Banner, BannerGetDto>().ReverseMap();

            //Brand Profiles
            CreateMap<Brand, BrandCreateDto>().ReverseMap();
            CreateMap<Brand, BrandUpdateDto>().ReverseMap();
            CreateMap<Brand, BrandDetailDto>().ReverseMap();
            CreateMap<Brand, BrandGetDto>().ReverseMap();

            //FeaturesBanner Profiles
            CreateMap<FeaturesBanner, FeaturesBannerCreateDto>().ReverseMap();
            CreateMap<FeaturesBanner, FeaturesBannerUpdateDto>().ReverseMap();
            CreateMap<FeaturesBanner, FeaturesBannerDetailDto>().ReverseMap();

            //Tag Profiles
            CreateMap<Tag, TagCreateDto>().ReverseMap();
            CreateMap<Tag, TagUpdateDto>().ReverseMap();
            CreateMap<Tag, TagGetDto>().ReverseMap();

            //Size Profiles
            CreateMap<Size, SizeCreateDto>().ReverseMap();
            CreateMap<Size, SizeUpdateDto>().ReverseMap();
            CreateMap<Size, SizeGetDto>().ReverseMap();

            //Color Profiles
            CreateMap<Color, ColorCreateDto>().ReverseMap();
            CreateMap<Color, ColorUpdateDto>().ReverseMap();
            CreateMap<Color, ColorGetDto>().ReverseMap();

            // AppUser Profiles
            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<AppUser, UserGetDto>().ReverseMap();

            //ReclamBanner Profiles
            CreateMap<ReclamBanner, ReclamBannerCreateDto>().ReverseMap();
            CreateMap<ReclamBanner, ReclamBannerUpdateDto>().ReverseMap();
            CreateMap<ReclamBanner, ReclamBannerDetailDto>().ReverseMap();

            //Basket Profiles
            CreateMap<CartItem, CartItemCreateDto>().ReverseMap();
            CreateMap<CartItem,CartItemDto>().ReverseMap();
            CreateMap<CartItem, CartGetDto>().ReverseMap();
            CreateMap<CartItem,CartDetailDto>().ReverseMap();

            //Order Profiles
            CreateMap<Order, OrderCreateDto>().ReverseMap();
            CreateMap<Order, OrderUpdateDto>().ReverseMap();
            CreateMap<Order, OrderGetDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ReverseMap();

            CreateMap<AppUser, UserGetDto>().ReverseMap();

            ///OrderItem Profiles
            CreateMap<OrderItem, OrderItemCreateDto>().ReverseMap().ForMember(x => x.Product, x => x.Ignore()).ForMember(x => x.TotalPrice, x => x.MapFrom(x => x.Product.CostPrice));
            CreateMap<OrderItem, OrderItemUpdateDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemGetDto>().ReverseMap();
            CreateMap<OrderItemCreateDto, CartItemDto>().ReverseMap();
            CreateMap<OrderItemGetDto, CartItemDto>().ReverseMap().ForMember(x => x.TotalPrice, x => x.MapFrom(x => x.Product.CostPrice));

            //Author Porfiles
            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorUpdateDto>().ReverseMap();

            //Blog Profiles
            CreateMap<Blog, BlogCreateDto>().ReverseMap();
            CreateMap<Blog, BlogUpdateDto>().ReverseMap();
        }
    }
}
