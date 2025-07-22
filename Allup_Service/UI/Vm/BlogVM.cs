

using Allup_Core.Entities;

namespace Allup_Service.UI.Vm
{
    public class BlogVM
    {

        public List<Blog> Blogs { get; set; } = new();
        // public List<Topic> Topics { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

    }

    public class BlogDetailVM
    {
        public Blog Blog { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Blog? PrevBlog { get; set; }
        public Blog? NextBlog { get; set; }
        public List<Blog> RelatedPosts { get; set; }
        public List<Blog> RecentBlogs { get; set; }

        // public List<BlogCommentGetDto> BlogComments { get; set; } = [];
        public bool IsAllowBlogComment { get; set; } = false;
        public int? NextBlogId { get; set; } // Növbəti Blog ID
        public int? PrevBlogId { get; set; } // Əvvəlki Blog ID
    }
}
