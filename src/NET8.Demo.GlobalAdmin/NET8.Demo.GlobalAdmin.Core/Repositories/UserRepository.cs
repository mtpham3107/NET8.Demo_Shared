using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.Shared;
using System.Linq.Expressions;

namespace NET8.Demo.GlobalAdmin.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly DbSet<User> _dbSet;
    private readonly GlobalAdminDbContext _context;

    public UserRepository(GlobalAdminDbContext context, ILogger<UserRepository> logger)
    {
        _dbSet = context.Set<User>();
        _logger = logger;
        _context = context;
    }

    public async ValueTask<User> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includes)
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
            _logger.LogError(ex, "UserRepository-GetByIdAsync-Exception: {Id}", id);
            throw;
        }
    }

    public async ValueTask<IEnumerable<User>> GetListAsync(
        Expression<Func<User, bool>> predicate = null,
        Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null,
        params Expression<Func<User, object>>[] includes)
    {
        try
        {
            return await FilterQuery(predicate, orderBy, includes).ToArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserRepository-GetListAsync-Exception: {predicate} - {orderBy} - {includes}", predicate, orderBy, includes);
            throw;
        }
    }

    public async ValueTask<PaginatedList<User>> GetListAsync(
        int? pageIndex, int? pageSize,
        Expression<Func<User, bool>> predicate = null,
        Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null,
        params Expression<Func<User, object>>[] includes)
    {
        try
        {
            return await FilterQuery(pageIndex, pageSize, predicate, orderBy, includes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserRepository-GetListAsync-Exception: {pageIndex} - {pageSize} - {predicate} - {orderBy} - {includes}", pageIndex, pageSize, predicate, orderBy, includes);
            throw;
        }
    }

    #region Private funtion
    private async Task<PaginatedList<User>> FilterQuery(
        int? pageIndex, int? pageSize,
        Expression<Func<User, bool>> predicate,
        Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy,
        params Expression<Func<User, object>>[] includes)
    {
        return await FilterQuery(predicate, orderBy, includes).ToPaginatedListAsync(pageIndex, pageSize);
    }

    private IQueryable<User> FilterQuery(
        Expression<Func<User, bool>> predicate,
        Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy,
        params Expression<Func<User, object>>[] includes)
    {
        IQueryable<User> query = _dbSet.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy.Compile().Invoke(query);

        return query;
    }
    #endregion
}
