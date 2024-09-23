using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Domain;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.Shared;
using NET8.Demo.Shared.Attributes;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.GlobalAdmin.Core.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
{
    private readonly ILogger<Repository<TEntity>> _logger;
    private readonly IStringLocalizer<Repository<TEntity>> _localizer;
    private readonly DbSet<TEntity> _dbSet;
    private readonly GlobalAdminDbContext _context;

    public Repository(GlobalAdminDbContext context, ILogger<Repository<TEntity>> logger, IStringLocalizer<Repository<TEntity>> localizer)
    {
        _dbSet = context.Set<TEntity>();
        _logger = logger;
        _localizer = localizer;
        _context = context;
    }

    public async ValueTask<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = true)
    {
        try
        {
            if (isAsNoTracking)
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            }
            else
            {
                return await _dbSet.FirstOrDefaultAsync(d => d.Id == id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetByIdAsync-Exception: {Id}", id);
            throw;
        }
    }

    public async ValueTask<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            var query = _dbSet.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(d => d.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetByIdAsync-Exception: {Id}", id);
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            return await FilterQuery(predicate, orderBy, includes).ToArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetListAsync-Exception: {predicate} - {includes} - {orderBy}", predicate, includes, orderBy);
            throw;
        }
    }

    public async ValueTask<PaginatedList<TEntity>> GetListAsync(
        int? pageIndex, int? pageSize,
        Expression<Func<TEntity, bool>> predicate = null,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            return await FilterQuery(pageIndex, pageSize, predicate, orderBy, includes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetListAsync-Exception: {pageIndex} - {pageSize} - {predicate} - {includes} - {orderBy}", pageIndex, pageSize, predicate, includes, orderBy);
            throw;
        }
    }

    public async ValueTask InsertAsync(Guid? userId, TEntity entity)
    {
        try
        {
            entity.CreatedBy = userId;
            entity.ModifiedBy = userId;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.IsActive = true;
            entity.IsDeleted = false;

            await _dbSet.AddAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-InsertAsync-Exception: {userId} - {entity}", userId, SerializeObject(entity));
            throw;
        }
    }

    public async ValueTask InsertAsync(Guid? userId, ICollection<TEntity> entities)
    {
        try
        {
            entities.ForEach(item =>
            {
                item.CreatedBy = userId;
                item.ModifiedBy = userId;
                item.CreatedDate = DateTime.UtcNow;
                item.ModifiedDate = DateTime.UtcNow;
                item.IsActive = true;
                item.IsDeleted = false;
            });

            await _dbSet.AddRangeAsync(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-InsertAsync-Exception: {userId} - {entities}", userId, SerializeObject(entities));
            throw;
        }
    }

    public void Delete(TEntity entity)
    {
        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Delete-Exception: {entity}", SerializeObject(entity));
            throw;
        }
    }

    public void Delete(ICollection<TEntity> entities)
    {
        try
        {
            _dbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Delete-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public void Update(Guid? userId, TEntity entity)
    {
        try
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = userId;

            var entry = _dbSet.Attach(entity);
            entry.State = EntityState.Modified;

            entry.Property(e => e.CreatedBy).IsModified = false;
            entry.Property(e => e.CreatedDate).IsModified = false;
            entry.Property(e => e.IsDeleted).IsModified = false;

            PropertyInfo[] properties = typeof(TEntity).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var noUpdateAttribute = propertyInfo.GetCustomAttribute<IgnoreUpdateAttribute>();
                if (noUpdateAttribute != null)
                {
                    entry.Property(propertyInfo.Name).IsModified = false;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Update-Exception: {userId} - {entity}", userId, SerializeObject(entity));
            throw;
        }
    }

    public void Update(Guid? userId, ICollection<TEntity> entities)
    {
        try
        {
            entities.ForEach(item =>
            {
                item.ModifiedDate = DateTime.UtcNow;
                item.ModifiedBy = userId;

                var entry = _dbSet.Attach(item);
                entry.State = EntityState.Modified;

                entry.Property(e => e.CreatedBy).IsModified = false;
                entry.Property(e => e.CreatedDate).IsModified = false;
                entry.Property(e => e.IsDeleted).IsModified = false;

                PropertyInfo[] properties = typeof(TEntity).GetProperties();
                foreach (var propertyInfo in properties)
                {
                    var noUpdateAttribute = propertyInfo.GetCustomAttribute<IgnoreUpdateAttribute>();
                    if (noUpdateAttribute != null)
                    {
                        entry.Property(propertyInfo.Name).IsModified = false;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Update-Exception: {userId} - {entities}", userId, SerializeObject(entities));
            throw;
        }
    }

    public void Modify(Guid? userId, TEntity entity, params Expression<Func<TEntity, object>>[] properties)
    {
        try
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = userId;

            var entry = _dbSet.Attach(entity);

            entry.Property(e => e.ModifiedDate).IsModified = true;
            entry.Property(e => e.ModifiedBy).IsModified = true;

            foreach (var property in properties)
            {
                entry.Property(property).IsModified = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Modify-Exception: {userId} - {entity}", userId, SerializeObject(entity));
            throw;
        }
    }

    public void Modify(Guid? userId, ICollection<TEntity> entities, params Expression<Func<TEntity, object>>[] properties)
    {
        try
        {
            entities.ForEach(entity =>
            {
                entity.ModifiedDate = DateTime.UtcNow;
                entity.ModifiedBy = userId;

                var entry = _dbSet.Attach(entity);
                var entityType = entity.GetType();

                entry.Property(e => e.ModifiedDate).IsModified = true;
                entry.Property(e => e.ModifiedBy).IsModified = true;

                foreach (var property in properties)
                {
                    entry.Property(property).IsModified = true;
                }
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-Modify-Exception: {userId} - {entities}", userId, SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask BulkInsertAsync(Guid? userId, ICollection<TEntity> entities)
    {
        try
        {
            entities.ForEach(item =>
            {
                item.CreatedDate = DateTime.UtcNow;
                item.CreatedBy = userId;
                item.ModifiedBy = userId;
                item.ModifiedDate = DateTime.UtcNow;
                item.IsActive = true;
                item.IsDeleted = false;
            });
            await _context.BulkInsertAsync(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-BulkInsertAsync-Exception: {userId} - {entities}", userId, SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask BulkUpdateAsync(Guid? userId, ICollection<TEntity> entities)
    {
        try
        {
            entities.ForEach(item =>
            {
                item.ModifiedBy = userId;
                item.ModifiedDate = DateTime.UtcNow;
            });

            await _context.BulkUpdateAsync(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-BulkUpdateAsync-Exception: {userId} - {entities}", userId, SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask BulkDeleteAsync(ICollection<TEntity> entities)
    {
        try
        {
            await _context.BulkDeleteAsync(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-BulkDeleteAsync-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask SoftDeleteAsync(Guid? userId, Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            TEntity entity = await FilterQuery(x => x.Id == id, null, includes).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(_localizer["Error.DataNotFound"]).WithData("id", id);
            entity.DeletedDate = DateTime.UtcNow;
            entity.DeletedBy = userId;
            entity.IsDeleted = true;

            _dbSet.Update(entity);

            foreach (var item in includes)
            {
                var relatedEntities = GetRelatedEntities(entity, item);

                foreach (var relatedEntity in relatedEntities)
                {
                    relatedEntity.DeletedDate = DateTime.UtcNow;
                    relatedEntity.DeletedBy = userId;
                    relatedEntity.IsDeleted = true;

                    _context.Update(relatedEntity);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Repository-SoftDeleteAsync-Exception: {Id}", id);

            throw;
        }
    }

    public async ValueTask<TEntity> GetDataSyncToRedisAsync(Guid id)
    {
        try
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetDataSyncToRedisAsync-Exception: {v}", id);
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> GetDataSyncToRedisAsync(IEnumerable<Guid> ids = null)
    {
        try
        {
            var query = _dbSet.AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .WhereIf(ids != null, x => ids.Contains(x.Id));

            return await query.ToArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository-GetDataSyncToRedisAsync-Exception: {ids}", SerializeObject(ids));
            throw;
        }
    }

    #region Private funtion
    private async ValueTask<PaginatedList<TEntity>> FilterQuery(
        int? pageIndex, int? pageSize,
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await FilterQuery(predicate, orderBy, includes).ToPaginatedListAsync(pageIndex, pageSize);
    }

    private IQueryable<TEntity> FilterQuery(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy.Compile().Invoke(query);

        return query;
    }

    private IEnumerable<EntityBase> GetRelatedEntities(TEntity entity, Expression<Func<TEntity, object>> relatedEntities)
    {
        var memberExpression = relatedEntities.Body as MemberExpression;
        var propertyInfo = memberExpression.Member as PropertyInfo;

        return propertyInfo.GetValue(entity) switch
        {
            IEnumerable<EntityBase> collection => collection,
            EntityBase singleEntity => [singleEntity],
            _ => []
        };
    }
    #endregion
}
