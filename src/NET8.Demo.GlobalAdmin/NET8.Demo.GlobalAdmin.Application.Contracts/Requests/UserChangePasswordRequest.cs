namespace NET8.Demo.GlobalAdmin.Application.Contracts.Requests;

public class UserChangePasswordRequest
{
    public Guid Id { get; set; }

    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }
}

