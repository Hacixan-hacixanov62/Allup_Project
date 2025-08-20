using Allup_Service.Dtos.BlogCommentDtos;
using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Service.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allup_Service.Service.IService
{
    public interface IBlogCommentService:IModifyService<BlogCommentCreateDto, BlogCommentUpdateDto>, IGetService<BlogCommentGetDto>
    {
        Task<List<BlogCommentGetDto>> GetBlogCommentsAsync(int blogId);
        Task<bool> CheckIsAllowBlogCommentAsync(int blogId);
        Task<bool> CreateReplyAsync(BlogCommentReplyDto dto, ModelStateDictionary ModelState);
        Task<List<BlogCommentGetDto>> GetComment(int id);
    }
}
