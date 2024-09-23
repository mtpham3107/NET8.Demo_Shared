using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Models;

public class RegisterViewModel : ExternalLoginViewModel
{
    [Required(ErrorMessage = "Register.FirstNameRequired")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Register.LastNameRequired")]
    public string LastName { get; set; }

    [DataType(DataType.PhoneNumber, ErrorMessage = "PhoneNumberInvalid")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "PasswordRequired")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Register.ConfirmPasswordRequied")]
    [Compare("Password", ErrorMessage = "Register.ConfirmPasswordInvalid")]
    public string ConfirmPassword { get; set; }
}
