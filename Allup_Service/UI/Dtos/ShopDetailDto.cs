

using Allup_Core.Entities;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;

namespace Allup_Service.UI.Dtos
{
    public class ShopDetailDto
    {
       //public List<CartItem> CartItems { get; set; } = null!;
        public ProductGetDto Product { get; set; } = null!;
        public List<BrandGetDto> Brands { get; set; } = null!;
        public List<CommentGetDto> Comments { get; set; } = [];
        public CommentCreateDto CommentCreateDto { get; set; } = new();

        public string SelectedCurrency { get; set; } = null!;
    }
}
