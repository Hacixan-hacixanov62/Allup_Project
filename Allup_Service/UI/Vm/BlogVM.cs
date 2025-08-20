

using Allup_Core.Entities;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.BlogCommentDtos;

namespace Allup_Service.UI.Vm
{
    public class BlogVM
    {

        public List<Blog> Blogs { get; set; } = new();
        // public List<Topic> Topics { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Blog? PrevBlog { get; set; }
        public Blog? NextBlog { get; set; }
        public int? NextBlogId { get; set; } // Növbəti Blog ID
        public int? PrevBlogId { get; set; } // Əvvəlki Blog ID
        public PaginationResponse<Blog> PaginatedBlogs { get; set; } = null!;

    }

    public class BlogDetailVM
    {
        public Blog Blog { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public List<Blog> RelatedPosts { get; set; } = new();
        public List<Blog> RecentBlogs { get; set; } = new();

        public BlogCommentCreateDto BlogCommentCreateDto { get; set; } = new();
        public List<BlogCommentGetDto> BlogComments { get; set; } = [];
        public bool IsAllowBlogComment { get; set; } = false;
    }
}
