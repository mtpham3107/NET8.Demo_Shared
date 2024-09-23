using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.Shared;
using System.Linq.Expressions;

namespace NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

public interface IUserService
{
    ValueTask<UserResponse> GetByIdAsync(Guid userId);

    ValueTask<IEnumerable<UserResponse>> GetByIdsAsync(ICollection<Guid> userId);

    ValueTask<UserResponse> GetByUserNameAsync(string userName);

    ValueTask<UserResponse> GetByEmailAsync(string email);

    ValueTask<IEnumerable<UserResponse>> GetAllAsync(Expression<Func<User, bool>> predicate = null, Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null, params Expression<Func<User, object>>[] includes);

    ValueTask<PaginatedList<UserResponse>> GetAllAsync(int? pageIndex, int? pageSize, Expression<Func<User, bool>> predicate = null, Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null, params Expression<Func<User, object>>[] includes);

    ValueTask<UserResponse> InsertAsync(UserInsertRequest model);

    ValueTask<UserResponse> UpdateAsync(UserUpdateRequest user);

    ValueTask<bool> DeleteAsync(Guid? userId);

    ValueTask<string> ResetPasswordAsync(string email);

    ValueTask<bool> ChangePasswordAsync(Guid? userId, string currentPassword, string newPassword);

    ValueTask<bool> CheckDuplicateEmailAsync(Guid? userId, string email);

    ValueTask<bool> CheckDuplicateUserNameAsync(Guid? userId, string userName);
}
