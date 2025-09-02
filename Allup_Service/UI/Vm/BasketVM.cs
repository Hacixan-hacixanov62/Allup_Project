

using Allup_Core.Entities;
using Allup_Service.Dtos.CartDtos;

namespace Allup_Service.UI.Vm
{
    public class BasketVM
    {
        public CartGetDto Cart { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = null!;
        public List<Brand> Brands { get; set; } = null!;
        public List<FeaturesBanner> FeaturesBanners { get; set; } = null!;
        //=============
        public int Id { get; set; }
        public int Count { get; set; }
        public string SelectedCurrency { get; set; } = null!;

    }
}
