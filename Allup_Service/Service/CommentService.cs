    using Allup_Core.Entities;
using Allup_Core.Enums;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class CommentService:ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public CommentService(ICommentRepository commentRepository,IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
            _commentRepository = commentRepository; 
            _mapper = mapper;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckIsAllowCommentAsync(int productId)
        {
            //var orders = await _orderService.GetAllAsync();

            var userId = _getUserId();


            var isExist = await _commentRepository.IsExistAsync(x => x.ProductId == productId && x.AppUserId == userId);

            if (isExist)
                return false;

            return true;
        }

        public async Task<bool> CreateAsync(CommentCreateDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            if (!_checkAuthorized())
                throw new UnAuthorizedException("NotFound Comment");

           // var orders = await _orderService.GetAllAsync();

            var userId = _getUserId();

            var comment = _mapper.Map<Comment>(dto);

            comment.AppUserId = userId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.CreatedBy = _contextAccessor.HttpContext?.User.Identity?.Name ?? "System"; // İstifadəçi adı və ya "System"
            comment.UpdatedBy = _contextAccessor.HttpContext?.User.Identity?.Name ?? "System"; // İstifadəçi adı və ya "System"


            await _commentRepository.CreateAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateReplyAsync(CommentReplyDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            if (!_checkAuthorized())
                throw new UnAuthorizedException("NotFound Comment");

            var userId = _getUserId();


            var parentComment = await _commentRepository.GetAsync(dto.ParentId);
            if (parentComment == null) throw new NotFoundException("Parent comment not found");

            var replyComment = _mapper.Map<Comment>(dto);
            replyComment.AppUserId = userId;
            replyComment.CreatedBy = _contextAccessor.HttpContext?.User.Identity?.Name ?? "System";
            replyComment.UpdatedBy = _contextAccessor.HttpContext?.User.Identity?.Name ?? "System";

            replyComment.Parent = parentComment;

            await _commentRepository.CreateAsync(replyComment);
            await _commentRepository.SaveChangesAsync();


            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _commentRepository.GetAsync(c => c.Id == id, include: q => q.Include(c => c.Children));

            if (comment is null)
                throw new NotFoundException("NotFound Comment");

            var userId = _getUserId();

            if (comment.AppUserId != userId && !_isAdmin())
                throw new UnAuthorizedException("NotFound Comment");

            await DeleteChildCommentsAsync(comment);



            await _commentRepository.Delete(comment);
            await _commentRepository.SaveChangesAsync();
        }
        private async Task DeleteChildCommentsAsync(Comment parentComment)
        {
            if (parentComment.Children != null && parentComment.Children.Any())
            {
                foreach (var child in parentComment.Children.ToList()) // Şərhləri təkrarlayın
                {
                    await DeleteChildCommentsAsync(child); // Rekursiv olaraq bağlı şərhləri silin
                    await _commentRepository.Delete(child); // Şərhi silin
                }
            }
        }

        public async Task<List<CommentGetDto>> GetComment(int productId)
        {
            var comments = await _commentRepository.GetAllAsync(x => x.ProductId == productId,
                                                   include: q => q.Include(c => c.AppUser));

            if (comments == null) comments = new List<Comment>(); // boş list qaytar

            var dtos = _mapper.Map<List<CommentGetDto>>(comments);

            return dtos ?? new List<CommentGetDto>(); // əmin ol ki, heç vaxt null qaytarmır
        }


        public Task<List<CommentGetDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CommentGetDto> GetAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task<List<CommentGetDto>> GetProductCommentsAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<CommentUpdateDto> GetUpdatedDtoAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(CommentUpdateDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }

        private string _getUserId()
        {
            return _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }

        private bool _checkAuthorized()
        {
            return _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }

        private bool _isAdmin()
        {
            return _contextAccessor.HttpContext?.User.IsInRole(IdentityRoles.Admin.ToString()) ?? false;
        }
    }
}
