
using Allup_Core.Entities;

namespace Allup_Service.UI.Vm
{
    public class ShopVM
    {
        public List<Product> Products { get; set; } = null!;
        public List<Category> Categories { get; set; } = null!;
        public List<Tag> Tags { get; set; } = null!;
        public List<Size> Sizes { get; set; } = null!;
        public List<Color> Colors { get; set; } = null!;
    }
}
