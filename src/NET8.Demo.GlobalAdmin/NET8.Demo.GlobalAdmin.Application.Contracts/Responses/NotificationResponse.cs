namespace NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

public class NotificationResponse
{
    public string Title { get; set; }

    public string Message { get; set; }

    public string Link { get; set; }

    public bool IsRead { get; set; }

    public UserResponse Sender { get; set; }

    public DateTime? CreatedDate { get; set; }
}
