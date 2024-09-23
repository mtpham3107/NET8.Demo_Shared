namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class Notification : EntityBase
{
    public Guid Recipient { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public string Link { get; set; }

    public bool IsRead { get; set; }
}
