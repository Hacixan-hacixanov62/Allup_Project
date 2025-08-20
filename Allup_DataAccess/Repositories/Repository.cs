using Allup_Core.Comman;
using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Allup_DataAccess.Repositories
{
    public class Repository<TEntity>:IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> Table { get; set; } // service hissesinde _appDbContext in yerine Tbale isletmek ucun istifade olunur
        public Repository(AppDbContext context)
        {
            _context = context;
            Table = _context.Set<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);

            return entityEntry.Entity;
        }

        public async Task Delete(TEntity entity)
        {
            Table.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> GetAll(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _getQueryWithIncludes(include);

            return query;
        }

        public async Task<List<TEntity>> GetAllAsync(
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _getQueryWithIncludes(include);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
        }
        public void Update(TEntity entity)
        {
            Table.Update(entity);
            _context.SaveChanges();
        }

        public IQueryable<TEntity> GetFilter(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = _getQueryWithIncludes(include);

            query = query.Where(expression);

            return query;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = _getQueryWithIncludes(include);

            var entity = await query.FirstOrDefaultAsync(expression);

            return entity;
        }

        public async Task<TEntity?> GetAsync(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = _getQueryWithIncludes(include);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = _getQueryWithIncludes(include);

            var result = await query.AnyAsync(expression);

            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> OrderBy(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression)
        {
            IQueryable<TEntity> result = query.OrderBy(expression);
            return result;
        }

        public IQueryable<TEntity> OrderByDescending(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression)
        {
            IQueryable<TEntity> result = query.OrderByDescending(expression);
            return result;
        }

        private IQueryable<TEntity> _getQueryWithIncludes(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = Table.AsQueryable();

            if (include is { })
                query = include(query);

            return query;
        }

        // Asagdakilar Messageye aidir


        public async Task<Chat?> GetChatWithUsersAndMessagesAsync(int chatId, string userId)
        {
            return await _context.Chats
                .Include(m => m.AppUserChats).ThenInclude(m => m.AppUser)
                .Include(m => m.Messages)
                .FirstOrDefaultAsync(m => m.Id == chatId && m.AppUserChats.Any(u => u.AppUserId == userId));
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
            return await _context.Chats
                .Include(m => m.AppUserChats)
                .Where(m => m.AppUserChats.Any(a => a.AppUserId == userId))
                .ToListAsync();
        }
    }
}
