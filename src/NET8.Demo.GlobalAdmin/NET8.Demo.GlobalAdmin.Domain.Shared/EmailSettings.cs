namespace NET8.Demo.GlobalAdmin;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
}
