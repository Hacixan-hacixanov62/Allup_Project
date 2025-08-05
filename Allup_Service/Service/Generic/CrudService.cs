using Allup_Core.Comman;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Allup_Service.Service.Generic
{
    public class CrudService<TEntity, TCreateDto, TUpdateDto, TDto> : ICrudService<TEntity, TCreateDto, TUpdateDto, TDto>
    where TEntity : BaseEntity

    {
        private readonly IRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public CrudService(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TDto> CreateAsync(TCreateDto entity)
        {
            var entityEntry = _mapper.Map<TEntity>(entity);
            await _repository.CreateAsync(entityEntry);
            return _mapper.Map<TDto>(entityEntry);
        }

        public async Task<TDto> DeleteAsync(int id)
        {
            var entity = await _repository.GetAsync(e => e.Id == id);
            if (entity == null) throw new NotFoundException("Entity not found");

            await _repository.Delete(entity);
            return _mapper.Map<TDto>(entity);
        }

        public async Task<List<TDto>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool enableTracking = true,
            int? take = null)
        {
            var entitiesQuery = _repository.GetAll(include);

            if (predicate != null)
                entitiesQuery = entitiesQuery.Where(predicate);

            if (orderBy != null)
                entitiesQuery = orderBy(entitiesQuery);

            if (take.HasValue)
                entitiesQuery = entitiesQuery.Take(take.Value);

            var entities = await entitiesQuery.ToListAsync();
            var dto = _mapper.Map<List<TDto>>(entities);
            return dto;
        }

        public async Task<TDto?> GetAsync(int id)
        {
            var entity = await _repository.GetAsync(e => e.Id == id);
            return _mapper.Map<TDto>(entity); ;
        }

        public virtual async Task<TDto?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            var entity = await _repository.GetAsync(predicate, include);
            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto> UpdateAsync(TUpdateDto entity)
        {
            var entityEntry = _mapper.Map<TEntity>(entity);
            _repository.Update(entityEntry);
            return _mapper.Map<TDto>(entityEntry);
        }

        public async Task SaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }

    }
}
