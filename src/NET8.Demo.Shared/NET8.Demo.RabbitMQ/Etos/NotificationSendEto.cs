namespace NET8.Demo.RabbitMQ.Etos;

public class NotificationSendEto
{
    public Guid Recipient { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public string Link { get; set; }

    public bool IsRead { get; set; } = false;

    public Guid CreatedBy { get; set; }
}
