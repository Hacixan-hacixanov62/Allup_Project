
using Allup_Core.Entities;

namespace Allup_Service.UI.Vm
{
    public class HomeVM
    {
        public List<Slider> Slider { get; set; } = null!;
        public List<Banner> Banner { get; set; } = null!;
        public List<FeaturesBanner> FeaturesBanner { get; set; } = null!;
        public List<Category> Categories { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
        public List<ReclamBanner> ReclamBanners { get; set; } = null!;

    }
}
