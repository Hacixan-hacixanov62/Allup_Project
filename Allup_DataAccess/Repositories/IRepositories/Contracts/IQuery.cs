using Allup_Core.Comman;

namespace Allup_DataAccess.Repositories.IRepositories.Contracts
{

    public interface IQuery<T> where T : BaseEntity
    {
        IQueryable<T> Query();
    }


}
