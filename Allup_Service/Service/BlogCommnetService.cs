using Allup_Core.Entities;
using Allup_Core.Enums;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BlogCommentDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class BlogCommnetService : IBlogCommentService
    {
        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlogService _blogService;
        private readonly IOrderService _orderService;
        public BlogCommnetService(IBlogCommentRepository blogCommentRepository, IBlogService blogService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IOrderService orderService)
        {
            _blogCommentRepository = blogCommentRepository;
            _blogService = blogService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _orderService = orderService;
        }
        public async Task<bool> CheckIsAllowBlogCommentAsync(int blogId)
        {
            var blog = await _blogService.GetAllAsync();

            var orders = await _orderService.GetAllAsync();

            var userId = _getUserId();


            var isExist = await _blogCommentRepository.IsExistAsync(x => x.BlogId == blogId && x.AppUserId == userId);

            if (isExist)
                return false;

            return true;
        }

        public async Task<bool> CreateAsync(BlogCommentCreateDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            if (!_checkAuthorized())
                throw new UnAuthorizedException("NotFound Comment");

            var blog = await _blogService.GetAllAsync();

            var orders = await _orderService.GetAllAsync();

            var userId = _getUserId();
            
            var comment = _mapper.Map<BlogComment>(dto);

            comment.AppUserId = userId;
            comment.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System"; // İstifadəçi adı və ya "System"
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System"; // İstifadəçi adı və ya "System"


            await _blogCommentRepository.CreateAsync(comment);
            await _blogCommentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateReplyAsync(BlogCommentReplyDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            if (!_checkAuthorized())
                throw new UnAuthorizedException("Blog Comment NotFound");

            var blog = await _blogService.GetAllAsync();

            var orders = await _orderService.GetAllAsync();

            var userId = _getUserId();

            var parentComment = await _blogCommentRepository.GetAsync(dto.ParentId);
            if (parentComment == null) throw new NotFoundException("Parent comment not found");

            var replyComment = _mapper.Map<BlogComment>(dto);
            replyComment.AppUserId = userId;
            replyComment.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";
            replyComment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";

            replyComment.Parent = parentComment;

            await _blogCommentRepository.CreateAsync(replyComment);
            await _blogCommentRepository.SaveChangesAsync();


            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _blogCommentRepository.GetAsync(id);

            if (comment is null)
                throw new NotFoundException("BlogComment NotFound");

            var userId = _getUserId();

            if (comment.AppUserId != userId && !_isAdmin())
                throw new UnAuthorizedException("BlogComment NotFound");

            await _blogCommentRepository.Delete(comment);
            await _blogCommentRepository.SaveChangesAsync();
        }


        public async Task<List<BlogCommentGetDto>> GetBlogCommentsAsync(int blogId)
        {
            var blogComments = await _blogCommentRepository.GetFilter(
                              x => x.BlogId == blogId && x.ParentId == null,
                              x => x.Include(x => x.AppUser)
                                   .Include(x => x.Children)
                                      .ThenInclude(child => child.AppUser)
                          ).ToListAsync();
            var dtos = _mapper.Map<List<BlogCommentGetDto>>(blogComments);

            return dtos;
        }


        private string _getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }

        private bool _checkAuthorized()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }

        private bool _isAdmin()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(IdentityRoles.Admin.ToString()) ?? false;
        }

        public Task<List<BlogCommentGetDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BlogCommentGetDto> GetAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task<BlogCommentUpdateDto> GetUpdatedDtoAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(BlogCommentUpdateDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }

    }
}
