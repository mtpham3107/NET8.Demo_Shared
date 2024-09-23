using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace NET8.Demo.GlobalAdmin.Controllers;

public class ErrorController : Controller
{
    private readonly IStringLocalizer<AccountController> _localizer;

    public ErrorController(IStringLocalizer<AccountController> localizer)
    {
        _localizer = localizer;
    }

    [Route("Error/{statusCode}", Name = "Error")]
    public IActionResult HttpStatusCodeHandler(int statusCode, string ReturnUrl)
    {
        ViewBag.ReturnUrl = ReturnUrl;
        switch (statusCode)
        {
            case 404:
                return View("NotFound");
            case 423:
                return View("LockedOut");
            case 500:
            default:
                return View("InternalServerError");
        }
    }
}
