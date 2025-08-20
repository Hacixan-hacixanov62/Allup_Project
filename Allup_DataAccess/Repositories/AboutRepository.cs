using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;

namespace Allup_DataAccess.Repositories
{
    public class AboutRepository : Repository<About>, IAboutRepository
    {
        public AboutRepository(AppDbContext context) : base(context)
        {
        }
    }
}
