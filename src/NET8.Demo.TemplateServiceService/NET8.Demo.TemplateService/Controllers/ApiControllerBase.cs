using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace NET8.Demo.TemplateService.Controllers;

[ApiController]
[Authorize()]
public abstract class ApiControllerBase : ControllerBase
{
    public Guid UserId => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId) ? userId : Guid.Empty;

    public string Token => Regex.Replace(Request.Headers.Authorization.ToString(), @"Bearer ", "", RegexOptions.IgnoreCase);

}
