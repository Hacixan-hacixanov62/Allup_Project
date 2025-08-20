
using Allup_Core.Entities;
using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Dtos.ProductDtos;

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
        public List<Blog> Blogs { get; set; } = null!;
        public int WishListCount { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        //public CommentCreateDto CommentCreateDto { get; set; } = new();
    }
}
