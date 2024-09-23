namespace NET8.Demo.GlobalAdmin.Application.Contracts.Requests;

public class UserInsertRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string AvatarUrl { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public bool EmailConfirmed { get; set; } = false;

    public string Role { get; set; }
}
