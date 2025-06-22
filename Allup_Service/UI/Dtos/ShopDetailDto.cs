

using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;

namespace Allup_Service.UI.Dtos
{
    public class ShopDetailDto
    {
        public ProductGetDto Product { get; set; } = null!;
        public List<BrandGetDto> Brands { get; set; } = null!;
        public bool IsAllowComment { get; set; } = false;

    }
}
