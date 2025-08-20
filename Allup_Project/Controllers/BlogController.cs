using Allup_DataAccess.DAL;
using Allup_Service.Dtos.BlogCommentDtos;
using Allup_Service.Extensions;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly IBlogCommentService _blogCommentService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public BlogController(IBlogService blogService, ICategoryService categoryService, AppDbContext context, ITagService tagService, IMapper mapper, IBlogCommentService blogCommentService)
        {
            _blogService = blogService;
            _categoryService = categoryService;
            _context = context;
            _tagService = tagService;
            _mapper = mapper;
            _blogCommentService = blogCommentService;
        }

        public async Task<IActionResult> Index(int? authorId ,string search, int page =1,int take=5)
        {
            var categories = await _categoryService.GetAllAsync();
            var blogs = await _blogService.GetAllAsync();
         

            if (!string.IsNullOrWhiteSpace(search))
            {
                blogs = blogs.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || b.Author.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                             .ToList();
            }
            var paginateBlogs = await _blogService.GetPaginateAsync(page, take,search);

            ViewBag.Search = search;

            BlogVM blogVM = new BlogVM  
            {
                Categories = categories,
                Blogs = blogs,
                PaginatedBlogs = paginateBlogs,
            };

            return View(blogVM);
        }
        
        [HttpGet("Blog/Detail/{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            var categories = await _categoryService.GetAllAsync();
            var blog = await _context.Blogs
                            .Include(m => m.Author)
                            .Include(m => m.BlogTags)
                            .ThenInclude(bt => bt.Tag)
                            .FirstOrDefaultAsync(m => m.Id == id);
            var allblogs = await _blogService.GetAllAsync();
            var cruntBlog = allblogs.FirstOrDefault(b => b.Id == id);
            var recentBlogs = await _context.Blogs
                                 .OrderByDescending(b => b.CreatedAt)
                                  .Take(5)
                                 .ToListAsync();
            //var tags = await _tagService.GetAllAsync();
            //var tagDtos = _mapper.Map<List<TagGetDto>>(tags);
            var relatedBlogs = await _context.Blogs
                .Where(b => b.CategoryId == blog.CategoryId && b.Id != blog.Id)
                .Take(3)
                .ToListAsync();


            if (blog == null) return NotFound();
            BlogDetailVM blogDetailVM = new BlogDetailVM
            {
                Blog = blog,
                Categories = categories,
                IsAllowBlogComment = true, // Assuming comments are allowed by default,
                RecentBlogs = recentBlogs,
                RelatedPosts = relatedBlogs,
            };


            return View(blogDetailVM);

        }



        [HttpPost]
        public async Task<IActionResult> CreateBlogComment([Bind(Prefix = "BlogCommentCreateDto")] BlogCommentCreateDto dto)
        {
            var result = await _blogCommentService.CreateAsync(dto,ModelState);

            string resultUrl =Request.GetReturnUrl();
            var comment = await _blogCommentService.GetComment(dto.BlogId);

            return PartialView("_BlogCommentPartial",comment);  

        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> ReplyComment(BlogCommentReplyDto dto)
        {

            var result = await _blogCommentService.CreateReplyAsync(dto, ModelState);

            string returnUrl = Request.GetReturnUrl();

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> DeleteBlogComment(int id)
        {
            await _blogCommentService.DeleteAsync(id);

            string returnUrl = Request.GetReturnUrl();

            return Redirect(returnUrl);
        }


    }
}






























