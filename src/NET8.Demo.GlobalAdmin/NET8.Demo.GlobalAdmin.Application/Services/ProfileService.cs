using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using System.Security.Claims;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class ProfileService : UserClaimsPrincipalFactory<User, Role>, IProfileService
{
    private readonly UserManager<User> _userManager;
    public ProfileService(UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        var identity = await base.GenerateClaimsAsync(user);

        // Add custom claims
        identity.AddClaim(new Claim("fullName", user.FullName ?? string.Empty));
        identity.AddClaim(new Claim("avatarUrl ", user.AvatarUrl ?? string.Empty));
        identity.AddClaim(new Claim("email", user.Email ?? string.Empty));
        identity.AddClaim(new Claim("phoneNumber", user.PhoneNumber ?? string.Empty));

        context.IssuedClaims.AddRange(identity.Claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        context.IsActive = (user != null);
    }
}