using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.Shared;
using System.Linq.Expressions;

namespace NET8.Demo.GlobalAdmin.Domain.IRepositories;

public interface IUserRepository
{
    ValueTask<User> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includes);

    ValueTask<IEnumerable<User>> GetListAsync(Expression<Func<User, bool>> predicate = null, Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null, params Expression<Func<User, object>>[] includes);

    ValueTask<PaginatedList<User>> GetListAsync(int? pageIndex, int? pageSize, Expression<Func<User, bool>> predicate = null, Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null, params Expression<Func<User, object>>[] includes);
}
