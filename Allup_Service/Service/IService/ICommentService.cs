

using Allup_Service.Dtos.CommentDtos;
using Allup_Service.Service.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allup_Service.Service.IService
{
    public interface ICommentService:IModifyService<CommentCreateDto ,CommentUpdateDto>,IGetService<CommentGetDto>
    {
        Task<List<CommentGetDto>> GetProductCommentsAsync(int productId);
        Task<bool> CheckIsAllowCommentAsync(int productId);

        Task<bool> CreateReplyAsync(CommentReplyDto dto, ModelStateDictionary ModelState);
        Task<List<CommentGetDto>> GetComment(int id);
    }
}
