using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.Shared;
using System.Net;
using System.Net.Mail;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly AppSettings _appSettings;
    private readonly IStringLocalizer<EmailService> _localizer;

    public EmailService(
        UserManager<User> userManager,
        ILogger<UserService> logger,
        IOptions<EmailSettings> emailSettings,
        IOptions<AppSettings> appSettings,
        IStringLocalizer<EmailService> localizer)
    {
        _emailSettings = emailSettings.Value;
        _userManager = userManager;
        _logger = logger;
        _appSettings = appSettings.Value;
        _localizer = localizer;
    }

    public async ValueTask SendEmailAsync(string toEmail, string subject, string message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async ValueTask SendEmailConfirmationAsync(string email, string returnUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new EntityNotFoundException(_localizer["Error.UserNotFound"]).WithData("email", email);

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_appSettings.JwtOptions.Authority}/Account/ConfirmEmail?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}&returnUrl={Uri.EscapeDataString(returnUrl ?? string.Empty)}";
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = _localizer["Register.ConfirmYourEmail"],
            Body = $"{_localizer["Hello"]} {user.FullName},<br/>{_localizer["Register.ConfirmEmailContent"]}: <a href='{confirmationLink}'>{_localizer["Register.ConfirmEmail"]}</a>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async ValueTask SendPasswordResetEmailAsync(string email, string returnUrl)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new EntityNotFoundException(_localizer["Error.UserNotFound"]).WithData("email", email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_appSettings.JwtOptions.Authority}/Account/NewPassword?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}&returnUrl={Uri.EscapeDataString(returnUrl ?? string.Empty)}";
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = _localizer["ResetPassword"],
                Body = $"{_localizer["ResetPasswordContent"]}: <a href='{resetLink}'>{_localizer["ResetPassword"]}</a>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EmailService-SendPasswordResetEmailAsync-Exception: {email}", email);
            throw;
        }
    }


    public async ValueTask SendAccountCreatedEmailAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new EntityNotFoundException(_localizer["Error.UserNotFound"]).WithData("email", email);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = _localizer["Email.ConfirmCreatedAccount"],
            Body = $@"{_localizer["Hello"]} {user.FullName},<br/>
                {_localizer["Email.ConfirmAccount"]}:<br/>
                - {_localizer["Account"]}: <b>{user.UserName}</b><br/>
                - {_localizer["Password"]}: <b>{password}</b><br/>
                {_localizer["Email.ConfirmAccountNote"]}.<br/>
                {_localizer["Email.Thank"]}!<br/>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
