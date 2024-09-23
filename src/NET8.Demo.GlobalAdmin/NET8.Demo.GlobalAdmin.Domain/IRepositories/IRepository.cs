using NET8.Demo.Shared;
using System.Linq.Expressions;

namespace NET8.Demo.GlobalAdmin.Domain.IRepositories;

public interface IRepository<TEntity> : IDataSyncRedisRepository<TEntity> where TEntity : EntityBase
{
    ValueTask<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = true);

    ValueTask<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

    ValueTask<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

    ValueTask<PaginatedList<TEntity>> GetListAsync(int? pageIndex, int? pageSize, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

    ValueTask InsertAsync(Guid? userId, TEntity entity);

    ValueTask InsertAsync(Guid? userId, ICollection<TEntity> entity);

    void Delete(TEntity entity);

    void Delete(ICollection<TEntity> entity);

    void Update(Guid? userId, TEntity entity);

    void Update(Guid? userId, ICollection<TEntity> entity);

    void Modify(Guid? userId, TEntity entity, params Expression<Func<TEntity, object>>[] properties);

    void Modify(Guid? userId, ICollection<TEntity> entities, params Expression<Func<TEntity, object>>[] properties);

    ValueTask BulkInsertAsync(Guid? userId, ICollection<TEntity> entities);

    ValueTask BulkUpdateAsync(Guid? userId, ICollection<TEntity> entities);

    ValueTask BulkDeleteAsync(ICollection<TEntity> entities);

    ValueTask SoftDeleteAsync(Guid? userId, Guid id, params Expression<Func<TEntity, object>>[] properties);
}