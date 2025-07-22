using Allup_DataAccess.DAL;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;
        public BlogController(IBlogService blogService, ICategoryService categoryService, AppDbContext context)
        {
            _blogService = blogService;
            _categoryService = categoryService;
            _context = context;
        }

        public async Task<IActionResult> Index(int? authorId)
        {
            var categories = await _categoryService.GetAllAsync();
            var blogs= await _blogService.GetAllAsync();

            BlogVM blogVM = new BlogVM
            {
                Categories = categories,
                Blogs = blogs
            };

            return View(blogVM);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var categories = await _categoryService.GetAllAsync();
            var blog = _context.Blogs.Include(m => m.Author).Include(m => m.Tag).FirstOrDefault(m => m.Id == id);
            var allblogs = await _blogService.GetAllAsync();
            var cruntBlog = allblogs.FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();
            BlogDetailVM blogDetailVM = new BlogDetailVM
            {
                Blog = blog,
                Categories = categories,
                IsAllowBlogComment = true // Assuming comments are allowed by default
            };


            return View(blogDetailVM);

        }


    }
}






























