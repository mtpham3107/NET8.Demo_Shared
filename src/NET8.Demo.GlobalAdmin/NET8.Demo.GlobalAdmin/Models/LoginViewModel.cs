using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Models;

public class LoginViewModel : ExternalLoginViewModel
{
    [Required(ErrorMessage = "UserNameRequired")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "PasswordRequired")]
    public string Password { get; set; }

    public bool RememberMe { get; set; }

    public string EmailConfirmation { get; set; }
}
