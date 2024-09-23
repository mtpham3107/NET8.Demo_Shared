using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.TemplateService.Domain.IUnitOfWorks;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace NET8.Demo.TemplateService.Application.Services;

public abstract class ServiceBase
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ILogger<ServiceBase> Logger;
    protected readonly IStringLocalizer<ServiceBase> Localizer;
    private readonly ClaimsPrincipal _user;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected ServiceBase(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ServiceBase> logger,
        IStringLocalizer<ServiceBase> localizer)
    {
        _httpContextAccessor = httpContextAccessor;
        _user = httpContextAccessor.HttpContext?.User;
        UnitOfWork = unitOfWork;
        Logger = logger;
        Localizer = localizer;
    }

    protected Guid UserId => Guid.TryParse(_user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId) ? userId : Guid.Empty;

    protected string RoleName => _user?.FindFirst(ClaimTypes.Role)?.Value;

    protected string Token => Regex.Replace(_httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString(), @"Bearer ", "", RegexOptions.IgnoreCase);

}

