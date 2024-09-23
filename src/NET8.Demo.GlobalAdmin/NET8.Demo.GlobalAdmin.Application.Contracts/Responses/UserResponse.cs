namespace NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

public class UserResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string AvatarUrl { get; set; }

    public ICollection<string> Roles { get; set; }
}
