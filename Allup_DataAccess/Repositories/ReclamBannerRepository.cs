

using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;

namespace Allup_DataAccess.Repositories
{
    public class ReclamBannerRepository : Repository<ReclamBanner>, IReclamBannerRepository
    {
        public ReclamBannerRepository(AppDbContext context) : base(context)
        {
        }
    }
}
