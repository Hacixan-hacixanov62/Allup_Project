using Allup_Core.Comman;
using Allup_Core.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Allup_DataAccess.Repositories.IRepositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync(
   Expression<Func<TEntity, bool>>? predicate = null,
   Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        IQueryable<TEntity> GetAll(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        void Update(TEntity entity);
        Task Delete(TEntity entity);
        Task<TEntity> CreateAsync(TEntity entity);
        IQueryable<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null); // Filtirle Olunmus melumatlari geri qaytarmaq
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null); // Tek bir obyekt getirmek
        Task<TEntity?> GetAsync(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null); // Obyektin olub olmamagini yoxlayir
        Task<int> SaveChangesAsync();
        IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression); // Artan Qaydada siraya salmaq
        IQueryable<TEntity> OrderByDescending(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression); // Azalan Qaydada siraya salmaq

        // Asagdakilar Message ye aidir
        Task<Chat?> GetChatWithUsersAndMessagesAsync(int chatId, string userId);
        Task<Message> AddMessageAsync(Message message);
        Task<List<Chat>> GetUserChatsAsync(string userId);
    }
}
