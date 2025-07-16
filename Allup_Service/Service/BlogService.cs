
using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BlogDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Allup_Service.Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IAuthorService _authorService;
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public BlogService(IMapper mapper, ICloudinaryService cloudinaryService, AppDbContext context, IAuthorService authorService, IBlogRepository blogRepository)
        {
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _context = context;
            _authorService = authorService;
            _blogRepository = blogRepository;
        }

        public async Task CreateAsync(BlogCreateDto blogCreateDto)
        {

            Blog blog = _mapper.Map<Blog>(blogCreateDto);
            blog.ImageUrl = await _cloudinaryService.FileCreateAsync(blogCreateDto.Image);

            //blog.BlogTopics = new List<BlogTopic>();


            //foreach (var topicId in blogCreateDto.TopicIds)
            //{
            //    BlogTopic blogTopic = new()
            //    {
            //        Blog = blog,
            //        TopicId = topicId
            //    };
            //    blog.BlogTopics.Add(blogTopic);
            //}

            await _blogRepository.CreateAsync(blog);
            await _blogRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _blogRepository.GetAsync(id, include: x => x.Include(c => c.Author));
            if (product == null)
            {
                throw new Exception("Blog tapılmadı");
            }

            await _blogRepository.Delete(product);
            await _blogRepository.SaveChangesAsync();
        }

        public async Task<Blog> DetailAsync(int id)
        {
            var blog = await _blogRepository.GetAsync(id, include: x => x.Include(c => c.Author));

            await _blogRepository.SaveChangesAsync();
            if (blog == null)
            {
                throw new Exception("Blog tapılmadı");
            }
            return blog;
        }

        public async Task EditAsync(int id, BlogUpdateDto blogUpdateDto)
        {
            var blog = await _blogRepository.GetAsync(id, include: x => x.Include(c => c.Author));

            if (blog == null)
                throw new Exception("Blog tapılmadı");

            var existBlog = await _blogRepository.GetAsync(blog.Id);
            if (existBlog is null)
            {
                throw new NotFoundException();
            }

            //foreach (var blogTopic in existBlog.BlogTopics.ToList())
            //{
            //    _context.BlogTopics.Remove(blogTopic);
            //}

            //if (blogUpdateDto.TopicIds is not null)
            //{
            //    foreach (var topicId in blogUpdateDto.TopicIds)
            //    {
            //        BlogTopic blogTopic = new() { Blog = existBlog, TopicId = topicId };
            //        existBlog.BlogTopics.Add(blogTopic);
            //    }
            //}

            existBlog = _mapper.Map(blogUpdateDto, existBlog);

            if (blogUpdateDto.Image is not null)
            {
                existBlog.ImageUrl = await _cloudinaryService.FileCreateAsync(blogUpdateDto.Image);
            }

            _blogRepository.Update(existBlog);
            await _blogRepository.SaveChangesAsync();
        }

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _blogRepository.GetAll()
                             .Include(c => c.Author)                           
                             .ToListAsync();

        }

        public Task<bool> IsExistAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
