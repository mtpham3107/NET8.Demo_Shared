using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.Shared;
using NET8.Demo.TemplateService.Application.Contracts.IServices;
using NET8.Demo.TemplateService.Domain;
using NET8.Demo.TemplateService.Domain.IRepositories;
using NET8.Demo.TemplateService.Domain.IUnitOfWorks;
using System.Linq.Expressions;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.TemplateService.Application.Services;

public class Service<TEntity> : ServiceBase, IService<TEntity> where TEntity : EntityBase
{
    private readonly IRepository<TEntity> _repository;
    private long orderCounter = 0;

    public Service(IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<Service<TEntity>> logger,
        IStringLocalizer<Service<TEntity>> localizer) : base(unitOfWork, httpContextAccessor, logger, localizer)
    {
        _repository = UnitOfWork.Repository<TEntity>();
    }

    public virtual async ValueTask<TEntity> GetByIdAsync(Guid id, bool isAsNoTracking = true)
    {
        try
        {
            return await _repository.GetByIdAsync(id, isAsNoTracking);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-GetByIdAsync-Exception: {Id}", id);
            throw;
        }
    }

    public virtual async ValueTask<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            return await _repository.GetByIdAsync(id, includes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-GetByIdAsync-Exception: {Id}", id);
            throw;
        }
    }

    public virtual async ValueTask<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            return await _repository.GetListAsync(predicate, orderBy, includes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-GetListAsync-Exception: {predicate} - {includes} - {orderBy}", predicate, includes, orderBy);
            throw;
        }
    }

    public virtual async ValueTask<PaginatedList<TEntity>> GetListAsync(int? pageIndex, int? pageSize, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            return await _repository.GetListAsync(pageIndex, pageSize, predicate, orderBy, includes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-GetListAsync-Exception: {pageIndex} - {pageSize} - {predicate} - {includes} - {orderBy}", pageIndex, pageSize, predicate, includes, orderBy);
            throw;
        }
    }

    public async ValueTask<TEntity> InsertAsync(TEntity entity)
    {
        try
        {
            await _repository.InsertAsync(UserId, entity);
            await UnitOfWork.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-InsertAsync-Exception: {entity}", SerializeObject(entity));
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entities)
    {
        try
        {
            await _repository.InsertAsync(UserId, entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-InsertAsync-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> BulkInsertAsync(IEnumerable<TEntity> entities)
    {
        try
        {
            await _repository.BulkInsertAsync(UserId, entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-BulkInsertAsync-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                throw new EntityNotFoundException(Localizer["Error.DataNotFound"]).WithData("id", id);
            }

            _repository.Delete(entity);
            await UnitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-DeleteAsync-Exception: {id}", id);
            throw;
        }
    }

    public async ValueTask<bool> DeleteAsync(IEnumerable<Guid> ids)
    {
        try
        {
            var entities = await _repository.GetListAsync(x => ids.Contains(x.Id));

            if (entities == null || !entities.Any())
            {
                throw new EntityNotFoundException(Localizer["Error.DataNotFound"]).WithData("ids", SerializeObject(ids));
            }

            _repository.Delete(entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-DeleteAsync-Exception: {ids}", SerializeObject(ids));
            throw;
        }
    }

    public async ValueTask<bool> BulkDeleteAsync(IEnumerable<Guid> ids)
    {
        try
        {
            var entities = await _repository.GetListAsync(x => ids.Contains(x.Id));

            if (entities == null || !entities.Any())
            {
                throw new EntityNotFoundException(Localizer["Error.DataNotFound"]).WithData("ids", SerializeObject(ids));
            }

            await _repository.BulkDeleteAsync(entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-BulkDeleteAsync-Exception: {ids}", SerializeObject(ids));
            throw;
        }
    }

    public async ValueTask<TEntity> SoftDeleteAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                throw new EntityNotFoundException(Localizer["Error.DataNotFound"]).WithData("id", id);
            }

            entity.IsActive = false;
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.DeletedBy = UserId;

            _repository.Modify(UserId, entity,
                x => x.IsActive,
                x => x.IsDeleted,
                x => x.DeletedDate,
                x => x.DeletedBy);

            await UnitOfWork.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-SoftDeleteAsync-Exception: {id}", id);
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> SoftDeleteAsync(IEnumerable<Guid> ids)
    {
        try
        {
            var entities = (await _repository.GetListAsync(x => ids.Contains(x.Id))).ToArray();

            if (entities == null || !entities.Any())
            {
                throw new EntityNotFoundException(Localizer["Error.DataNotFound"]).WithData("ids", SerializeObject(ids));
            }

            entities.ForEach(entity =>
            {
                entity.IsActive = false;
                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;
                entity.DeletedBy = UserId;
            });

            _repository.Modify(UserId, entities,
                x => x.IsActive,
                x => x.IsDeleted,
                x => x.DeletedDate,
                x => x.DeletedBy);

            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-SoftDeleteAsync-Exception: {ids}", SerializeObject(ids));
            throw;
        }
    }

    public async ValueTask<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
            _repository.Update(UserId, entity);
            await UnitOfWork.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-UpdateAsync-Exception: {entity}", SerializeObject(entity));
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
    {
        try
        {
            _repository.Update(UserId, entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-UpdateAsync-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities)
    {
        try
        {
            await _repository.BulkUpdateAsync(UserId, entities.ToArray());
            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-BulkUpdateAsync-Exception: {entities}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask<TEntity> ModifyAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
    {
        try
        {
            _repository.Modify(UserId, entity, properties);
            await UnitOfWork.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-ModifyAsync-Exception: {entity}", SerializeObject(entity));
            throw;
        }
    }

    public async ValueTask<IEnumerable<TEntity>> ModifyAsync(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] properties)
    {
        try
        {
            _repository.Modify(UserId, entities.ToArray(), properties);
            await UnitOfWork.SaveChangesAsync();
            return entities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-ModifyAsync-Exception: {entity}", SerializeObject(entities));
            throw;
        }
    }

    public async ValueTask SaveChangesAsync()
    {
        try
        {
            await UnitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-SaveChangesAsync-Exception");
            throw;
        }
    }

    public string GenerateUniqueCode(string prefix)
    {
        try
        {
            long currentCounter = Interlocked.Increment(ref orderCounter);
            string timestamp = DateTime.Now.ToString("yyMMddHHmmssfff");
            return $"{prefix}-{timestamp}{currentCounter}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Service-GetNewCode-Exception: {prefix}", prefix);
            throw;
        }
    }
}
