using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace NET8.Demo.GlobalAdmin.Controllers.Api;

[Route("api/localizations")]
[ApiController]
public class LocalizationController : ControllerBase
{
    private readonly IStringLocalizer<LocalizationController> _localizer;

    public LocalizationController(IStringLocalizer<LocalizationController> localizer)
    {
        _localizer = localizer;
    }

    [HttpGet("get-localized")]
    public IActionResult GetLocalizedStrings()
    {
        return Ok(_localizer.GetAllStrings(true).ToDictionary(s => s.Name, s => _localizer[s.Name].Value));
    }

    [HttpGet("change-culture")]
    public IActionResult ChangeCulture(string culture)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            };
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                cookieOptions);
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }
}
