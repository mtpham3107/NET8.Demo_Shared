namespace NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

public interface IEmailService
{
    ValueTask SendEmailAsync(string toEmail, string subject, string message);

    ValueTask SendEmailConfirmationAsync(string email, string returnUrl);

    ValueTask SendPasswordResetEmailAsync(string email, string returnUrl);

    ValueTask SendAccountCreatedEmailAsync(string email, string password);
}
