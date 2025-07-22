using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;

namespace Allup_DataAccess.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
