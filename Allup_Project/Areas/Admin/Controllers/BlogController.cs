using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BlogDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly IBlogRepository _blogRepository;
        private readonly IAuthorService _authorService;
        private readonly AppDbContext _context;
        public BlogController(ICategoryService categoryService, IBlogService blogService, IMapper mapper, ITagService tagService, IBlogRepository blogRepository, IAuthorService authorService, AppDbContext context)
        {
            _categoryService = categoryService;
            _blogService = blogService;
            _mapper = mapper;
            _tagService = tagService;
            _blogRepository = blogRepository;
            _authorService = authorService;
            _context = context;
        }
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page = 1, int take = 4)
        {
            try
            {
                var blogs = _blogRepository.GetAll().Include(m => m.Author).Include(m => m.Tag).Include(m => m.Category).OrderByDescending(m => m.CreatedAt).AsQueryable();
                PaginatedList<Blog> paginatedList = PaginatedList<Blog>.Create(blogs, take, page);

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Create()
        {
            var tag = await _tagService.GetAllAsync();
            var category = await _categoryService.GetAllAsync();
            var author = await _authorService.GetAllAsync();

            ViewBag.Tags = new SelectList(tag, nameof(Tag.Id), nameof(Tag.Name));
            ViewBag.Authors = new SelectList(author, nameof(Author.Id), nameof(Author.Name));
            ViewBag.Categories = new SelectList(category, nameof(Category.Id), nameof(Category.Name));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BlogCreateDto blogCreateDto)
        {
            ViewBag.Author = await _authorService.GetAllAsync();
            ViewBag.Tag = await _tagService.GetAllAsync();
            ViewBag.Category = await _categoryService.GetAllAsync();

            if (!ModelState.IsValid)
            {
                return View(blogCreateDto);
            }

            var isExistAuth = await _context.Authors.AnyAsync(x => x.Id == blogCreateDto.AuthorId);
            if (!isExistAuth)
            {
                ModelState.AddModelError("AuthorId", "Author is not found");
                return View(blogCreateDto);
            }

            var isExistCat = await _context.Categories.AnyAsync(x => x.Id == blogCreateDto.CategoryId);
            if (!isExistCat)
            {
                ModelState.AddModelError("CategoryId", "Category is not found");
                return View(blogCreateDto);
            }

            //foreach (var topic in blogCreateDto.TopicIds)
            //{
            //    var isExistTopic = await _context.Topics.AnyAsync(x => x.Id == topic);
            //    if (!isExistTopic)
            //    {
            //        ModelState.AddModelError("TopicIds", "Topic is not found");
            //        return View(blogCreateDto);
            //    }
            //}

            if (_context.Blogs.Any(x => x.Title == blogCreateDto.Title))
            {
                ModelState.AddModelError("", "Blog already exists");
                return View(blogCreateDto);
            }

            await _blogService.CreateAsync(blogCreateDto);
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id < 1)
            {
                return NotFound();
            }

            var author = await _authorService.GetAllAsync();
            var tag = await _tagService.GetAllAsync();
            var category = await _categoryService.GetAllAsync();

            ViewBag.Tags = new SelectList(tag, nameof(Tag.Id), nameof(Tag.Name));
            ViewBag.Authors = new SelectList(author, nameof(Author.Id), nameof(Author.Name));
            ViewBag.Categories = new SelectList(category, nameof(Category.Id), nameof(Category.Name));

            var blog = await _blogService.GetAllAsync();

            if (blog == null)
            {
                return NotFound();
            }

            BlogUpdateDto blogUpdateDto = _mapper.Map<BlogUpdateDto>(blog.FirstOrDefault(x => x.Id == id));

            return View(blogUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BlogUpdateDto blogUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = new SelectList(await _authorService.GetAllAsync(), nameof(Author.Id), nameof(Author.Name));
                ViewBag.Tags = new SelectList(await _tagService.GetAllAsync(), nameof(Tag.Id), nameof(Tag.Name));
                ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), nameof(Category.Id), nameof(Category.Name));

                return View(blogUpdateDto);
            }

            var isExistAuth = await _blogRepository.IsExistAsync(m => m.Title == blogUpdateDto.Title && m.Id != id);

            if (isExistAuth)
            {
                ViewBag.Tags = new SelectList(await _tagService.GetAllAsync(), nameof(Tag.Id), nameof(Tag.Name));
                ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), nameof(Category.Id), nameof(Category.Name));
                ModelState.AddModelError("", "Blog already exists");
                return View(blogUpdateDto);
            }

            var isExistTag = await _blogRepository.IsExistAsync(m => m.TagId == blogUpdateDto.TagId && m.Id != id);
            if (!isExistTag)
            {
                ViewBag.Authors = new SelectList(await _authorService.GetAllAsync(), nameof(Author.Id), nameof(Author.Name));
                ViewBag.Tags = new SelectList(await _tagService.GetAllAsync(), nameof(Tag.Id), nameof(Tag.Name));

                ModelState.AddModelError("TagId", "Tag is not found");
                return View(blogUpdateDto);
            }

            await _blogService.EditAsync(id, blogUpdateDto);
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _blogService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        [HttpGet("admin/Blog/Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var blog = await _blogService.DetailAsync(id);
                if (blog == null)
                {
                    return NotFound();
                }
                return View(blog);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
