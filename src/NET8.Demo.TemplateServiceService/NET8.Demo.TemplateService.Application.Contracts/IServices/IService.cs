using NET8.Demo.Shared;
using NET8.Demo.TemplateService.Domain;
using System.Linq.Expressions;

namespace NET8.Demo.TemplateService.Application.Contracts.IServices;

public interface IService<TEntity> where TEntity : EntityBase
{
    ValueTask<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = true);

    ValueTask<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

    ValueTask<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

    ValueTask<PaginatedList<TEntity>> GetListAsync(int? pageIndex, int? pageSize, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

    ValueTask<TEntity> InsertAsync(TEntity entity);

    ValueTask<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entity);

    ValueTask<IEnumerable<TEntity>> BulkInsertAsync(IEnumerable<TEntity> entities);

    ValueTask<bool> DeleteAsync(Guid id);

    ValueTask<bool> DeleteAsync(IEnumerable<Guid> ids);

    ValueTask<bool> BulkDeleteAsync(IEnumerable<Guid> ids);

    ValueTask<TEntity> SoftDeleteAsync(Guid id);

    ValueTask<IEnumerable<TEntity>> SoftDeleteAsync(IEnumerable<Guid> ids);

    ValueTask<TEntity> UpdateAsync(TEntity entity);

    ValueTask<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entity);

    ValueTask<IEnumerable<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities);

    ValueTask<TEntity> ModifyAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties);

    ValueTask<IEnumerable<TEntity>> ModifyAsync(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);

    ValueTask SaveChangesAsync();

    string GenerateUniqueCode(string prefix);
}
