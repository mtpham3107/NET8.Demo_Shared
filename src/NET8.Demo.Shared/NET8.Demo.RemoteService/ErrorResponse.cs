namespace NET8.Demo.RemoteServices;
public class ErrorResponse
{
    public ErrorDetail Error { get; set; }
}

public class ErrorDetail
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string Detail { get; set; }
}
