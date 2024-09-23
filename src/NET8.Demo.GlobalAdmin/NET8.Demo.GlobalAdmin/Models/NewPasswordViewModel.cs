using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Models;

public class NewPasswordViewModel : ExternalLoginViewModel
{
    [Required(ErrorMessage = "PasswordRequired")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Register.ConfirmPasswordRequied")]
    [Compare("Password", ErrorMessage = "Register.ConfirmPasswordInvalid")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }

    public string Email { get; set; }
}
