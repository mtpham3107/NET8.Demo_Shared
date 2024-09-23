using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;
using NET8.Demo.Shared;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class UserService : ServiceBase, IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public UserService(
        IMapper mapper,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger<UserService> logger,
        IUserRepository userRepository,
        IStringLocalizer<UserService> localizer,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor, logger, localizer)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async ValueTask<UserResponse> GetByIdAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userId", userId);

            var result = _mapper.Map<UserResponse>(user);
            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetByIdAsync-Exception: {userId}", userId);
            throw;
        }
    }

    public async ValueTask<IEnumerable<UserResponse>> GetByIdsAsync(ICollection<Guid> userIds)
    {
        try
        {
            var users = await _userRepository.GetListAsync(x => userIds.Contains(x.Id));

            if (users == null || !users.Any()) throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userIds", SerializeObject(userIds));

            var userResponses = new ConcurrentBag<UserResponse>();

            var task = users.Select(async user =>
            {
                var userResponse = _mapper.Map<UserResponse>(user);
                userResponse.Roles = await _userManager.GetRolesAsync(user);
                userResponses.Add(userResponse);
            }).ToArray();

            await Task.WhenAll(task);

            return userResponses;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetByIdsAsync-Exception: {userIds}", SerializeObject(userIds));
            throw;
        }
    }

    public async ValueTask<UserResponse> GetByUserNameAsync(string userName)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userName", userName);

            var result = _mapper.Map<UserResponse>(user);
            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetByUserNameAsync-Exception: {userName}", userName);
            throw;
        }
    }

    public async ValueTask<UserResponse> GetByEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("email", email);

            var result = _mapper.Map<UserResponse>(user);
            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetByEmailAsync-Exception: {email}", email);
            throw;
        }
    }

    public async ValueTask<IEnumerable<UserResponse>> GetAllAsync(
        Expression<Func<User, bool>> predicate = null,
        Expression<Func<IQueryable<User>,
        IOrderedQueryable<User>>> orderBy = null,
        params Expression<Func<User, object>>[] includes)
    {
        try
        {
            return _mapper.Map<IEnumerable<UserResponse>>(await _userRepository.GetListAsync(predicate, orderBy, includes));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetAllAsync-Exception: {predicate} - {includes} - {orderBy}", predicate, includes, orderBy);
            throw;
        }
    }

    public async ValueTask<PaginatedList<UserResponse>> GetAllAsync(
        int? pageIndex, int? pageSize,
        Expression<Func<User, bool>> predicate = null,
        Expression<Func<IQueryable<User>, IOrderedQueryable<User>>> orderBy = null,
        params Expression<Func<User, object>>[] includes)
    {
        try
        {
            return _mapper.Map<PaginatedList<UserResponse>>(await _userRepository.GetListAsync(pageIndex, pageSize, predicate, orderBy, includes));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-GetAll-Exception: {pageIndex} - {pageSize} - {predicate} - {includes} - {orderBy}", pageIndex, pageSize, predicate, includes, orderBy);
            throw;
        }
    }

    public async ValueTask<UserResponse> InsertAsync(UserInsertRequest model)
    {
        try
        {
            var user = _mapper.Map<UserInsertRequest, User>(model);
            user.UserName = model.UserName ?? model.Email;
            user.CreatedBy = UserId;
            user.ModifiedBy = UserId;
            user.CreatedDate = DateTime.UtcNow;
            user.ModifiedDate = DateTime.UtcNow;
            user.EmailConfirmed = model.EmailConfirmed;

            if (await CheckDuplicateEmailAsync(null, user.Email))
            {
                throw new BusinessException(ErrorCode.InternalServerError, Localizer["Error.EmailExists"]).WithData("email", user.Email);
            }

            if (await CheckDuplicateUserNameAsync(null, user.UserName))
            {
                throw new BusinessException(ErrorCode.InternalServerError, Localizer["Error.UsernameExists"]).WithData("userName", user.UserName);
            }

            user.LockoutEnabled = true;
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new BusinessException(ErrorCode.InternalServerError, string.Join(", ", result.Errors.Select(error => error.Description)));
            }

            switch (model.Role)
            {
                case SharedConstant.Role.Supplier:
                    await _userManager.AddToRoleAsync(user, SharedConstant.Role.Supplier);
                    break;
                case SharedConstant.Role.Affiliate:
                    await _userManager.AddToRoleAsync(user, SharedConstant.Role.Affiliate);
                    break;
                default:
                    await _userManager.AddToRoleAsync(user, SharedConstant.Role.Customer);
                    break;
            }

            return _mapper.Map<UserResponse>(user);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-Create-Exception: {user}", model);
            throw;
        }
    }

    public async ValueTask<UserResponse> UpdateAsync(UserUpdateRequest model)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user != null)
            {
                if (await CheckDuplicateEmailAsync(user.Id, user.Email))
                {
                    throw new BusinessException(ErrorCode.InternalServerError, Localizer["Error.EmailExists"]).WithData("email", user.Email);
                }

                if (await CheckDuplicateUserNameAsync(user.Id, user.UserName))
                {
                    throw new BusinessException(ErrorCode.InternalServerError, Localizer["Error.UsernameExists"]).WithData("userName", user.UserName);
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.AvatarUrl = model.AvatarUrl;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = UserId;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new BusinessException(ErrorCode.InternalServerError, string.Join(", ", result.Errors.Select(error => error.Description)));
                }

                return _mapper.Map<UserResponse>(user);
            }
            else
            {
                throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userId", model.Id);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-Update-Exception: {user}", model);
            throw;
        }
    }

    public async ValueTask<bool> DeleteAsync(Guid? userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                user.IsActive = false;
                user.IsDeleted = true;
                user.DeletedBy = UserId;
                user.DeletedDate = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new BusinessException(ErrorCode.BadRequest, string.Join(", ", result.Errors.Select(error => error.Description)));
                }
            }
            else
            {
                throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userId", userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-Delete-Exception: {userId}", userId);
            throw;
        }
    }

    public async ValueTask<string> ResetPasswordAsync(string email)
    {
        await _emailService.SendPasswordResetEmailAsync(email, string.Empty);
        return string.Format(Localizer["EmailPasswordMessageFull"], email);
    }

    public async ValueTask<bool> ChangePasswordAsync(Guid? userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId?.ToString());
            if (user != null)
            {
                if (!await _userManager.CheckPasswordAsync(user, currentPassword))
                {
                    throw new BusinessException(ErrorCode.InternalServerError, Localizer["PasswordInvalid"]);
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!result.Succeeded)
                {
                    throw new BusinessException(ErrorCode.InternalServerError, string.Join(", ", result.Errors.Select(error => error.Description)));
                }
            }
            else
            {
                throw new EntityNotFoundException(Localizer["Error.UserNotFound"]).WithData("userId", userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-ChangePasswordAsync-Exception: {userId}", userId);
            throw;
        }
    }

    public async ValueTask<bool> CheckDuplicateEmailAsync(Guid? userId, string email)
    {
        try
        {
            var currentUser = await _userManager.FindByIdAsync(userId?.ToString());
            var existingUser = await _userManager.FindByEmailAsync(email?.ToString());
            return existingUser != null && existingUser.Id != currentUser?.Id;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-CheckDuplicateEmailAsync-Exception: {userId} - {email}", userId, email);
            throw;
        }
    }

    public async ValueTask<bool> CheckDuplicateUserNameAsync(Guid? userId, string userName)
    {
        try
        {
            var currentUser = await _userManager.FindByIdAsync(userId?.ToString());
            var existingUser = await _userManager.FindByNameAsync(userName?.ToString());
            return existingUser != null && existingUser.Id != currentUser?.Id;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "UserService-CheckDuplicateUserNameAsync-Exception: {userId} - {userName}", userId, userName);
            throw;
        }
    }
}
