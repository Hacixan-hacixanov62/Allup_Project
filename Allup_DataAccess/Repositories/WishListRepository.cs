using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;

namespace Allup_DataAccess.Repositories
{
    public class WishListRepository : Repository<WishlistItem>, IWishListRepository
    {
        public WishListRepository(AppDbContext context) : base(context)
        {
        }
    }
}
