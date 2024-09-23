namespace NET8.Demo.GlobalAdmin.Models;

public class EmailConfirmationViewModel : ExternalLoginViewModel
{
    public string Title { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
}
