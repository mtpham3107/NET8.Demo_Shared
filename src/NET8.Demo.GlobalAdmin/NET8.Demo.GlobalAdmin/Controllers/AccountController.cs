using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Models;
using NET8.Demo.Shared;

namespace NET8.Demo.GlobalAdmin.Controllers;

public class AccountController : Controller
{
    private SignInManager<User> _signInManager;
    private readonly IUserService _service;
    private UserManager<User> _userManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IStringLocalizer<AccountController> _localizer;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserService service,
        IStringLocalizer<AccountController> localizer,
        IEmailService emailService,
        ILogger<AccountController> logger,
        IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _service = service;
        _interaction = interaction;
        _localizer = localizer;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult Login(string returnUrl)
    {
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || user.IsDeleted || !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, _localizer["Login.InvalidLoginAttempt"]);
                    model.ReturnUrl = returnUrl;
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    return RedirectToRoute("Error", new { statusCode = 423, ReturnUrl = returnUrl });
                }
                else
                {
                    bool requireConfirmedEmail = _userManager.Options.SignIn.RequireConfirmedEmail;
                    if (requireConfirmedEmail)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError(string.Empty, string.Format(_localizer["EmailNotConfirmed"], user.Email));
                            return View(new LoginViewModel { ReturnUrl = returnUrl, EmailConfirmation = user.Email });
                        }
                    }

                    ModelState.AddModelError(string.Empty, _localizer["Login.InvalidLoginAttempt"]);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        model.ReturnUrl = returnUrl;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);

        await _signInManager.SignOutAsync();

        if (!string.IsNullOrWhiteSpace(logoutContext.PostLogoutRedirectUri))
        {
            return Redirect(logoutContext.PostLogoutRedirectUri);
        }

        return NoContent();
    }

    [HttpGet]
    public ActionResult Register(string returnUrl)
    {
        var model = new RegisterViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var userRequest = new UserInsertRequest
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
            };

            try
            {
                await _service.InsertAsync(userRequest);
                var user = await _userManager.FindByEmailAsync(userRequest.Email);
                await _userManager.AddToRoleAsync(user, SharedConstant.Role.Customer);

                bool requireConfirmedEmail = _userManager.Options.SignIn.RequireConfirmedEmail;
                if (requireConfirmedEmail)
                {
                    await _emailService.SendEmailConfirmationAsync(model.Email, returnUrl);

                    return View("EmailConfirmation", new EmailConfirmationViewModel()
                    {
                        Title = _localizer["EmailConfirmation"],
                        Email = model.Email,
                        Message = _localizer["EmailConfirmationMessage"],
                        ReturnUrl = model.ReturnUrl
                    });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        model.ReturnUrl = returnUrl;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailViewModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new BusinessException(ErrorCode.BadRequest, _localizer["Error.UserNotFound"]);
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new BusinessException(ErrorCode.BadRequest, _localizer["EmailAlreadyConfirmed"]);

            }

            await _emailService.SendEmailConfirmationAsync(model.Email, model.ReturnUrl);

            return Ok(new EmailConfirmationViewModel()
            {
                Title = _localizer["EmailConfirmation"],
                Email = model.Email,
                Message = string.Format(_localizer["EmailConfirmationMessageFull"], model.Email),
                ReturnUrl = model.ReturnUrl
            });


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AccountController-ResendConfirmationEmail-Exception: {email}", model.Email);
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string email, string token, string returnUrl)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AccountController-ConfirmEmail-Exception: {email}", email);
        }

        return RedirectToRoute("Error", new { statusCode = 500, ReturnUrl = returnUrl });
    }

    [HttpGet]
    public ActionResult ResetPassword(string returnUrl)
    {
        var model = new ForgotPasswordViewModel()
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ForgotPasswordViewModel model, string returnUrl)
    {
        try
        {
            await _emailService.SendPasswordResetEmailAsync(model.Email, returnUrl);

            return View("EmailConfirmation", new EmailConfirmationViewModel()
            {
                Title = _localizer["ResetPasswordConfirmation"],
                Email = model.Email,
                Message = _localizer["ResetPasswordConfirmationMessage"],
                ReturnUrl = model.ReturnUrl
            });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        model.ReturnUrl = returnUrl;
        return View(model);
    }

    [HttpGet]
    public ActionResult EmailConfirmation(string title, string email, string message, string returnUrl)
    {
        var model = new EmailConfirmationViewModel()
        {
            Title = title,
            Email = email,
            Message = message,
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpGet]
    public ActionResult Confirmation(string title, string message, string returnUrl)
    {
        var model = new ConfirmationViewModel()
        {
            Title = title,
            Message = message,
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpGet]
    public ActionResult NewPassword(string email, string token, string returnUrl)
    {
        var model = new NewPasswordViewModel
        {
            Token = token,
            Email = email,
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> NewPassword(NewPasswordViewModel model, string email, string token, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    return View("Confirmation", new ConfirmationViewModel()
                    {
                        Title = _localizer["PasswordChanged"],
                        Message = _localizer["LoginToContinue"],
                        ReturnUrl = model.ReturnUrl
                    });
                }
                else if (result.Errors.Any(e => e.Code == "InvalidToken"))
                {
                    ModelState.AddModelError(string.Empty, _localizer["Error.InvalidToken"]);
                }
                else
                {
                    AddErrors(result);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, _localizer["Error.UserNotFound"]);
            }
        }

        model.Token = token;
        model.Email = email;
        model.ReturnUrl = returnUrl;
        return View(model);
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }
    }
}