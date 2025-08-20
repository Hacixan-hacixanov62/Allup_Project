
using Allup_Core.Entities;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;

namespace Allup_Service.UI.Dtos
{
    public class ShopDto:ShopPaginateDto
    {
        public List<ProductGetDto> Products { get; set; } = new();
        public List<CategoryGetDto> Categories { get; set; } = new();
        public List<SizeGetDto> Sizes { get; set; } = new();
        public List<TagGetDto> Tags { get; set; } = new();
        public List<ColorGetDto> Colors { get; set; } = new();

        public List<int>? SelectedColor { get; set; }

        public List<int>? SelectedBrand { get; set; }

        public List<int>? SelectedSize { get; set; }

        public ProductGetDto Product { get; set; } = null!;


        public int Size { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }


        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public decimal? SelectedMinPrice { get; set; }

        public decimal? SelectedMaxPrice { get; set; }

        public List<FeaturesBanner> FeaturesBanners { get; set; } = null!;
        public List<Brand> Brands { get; set; } = null!;

    }
}
