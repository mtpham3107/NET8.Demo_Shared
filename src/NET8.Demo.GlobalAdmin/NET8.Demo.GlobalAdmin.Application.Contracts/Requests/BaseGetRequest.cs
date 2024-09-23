namespace NET8.Demo.GlobalAdmin.Application.Contracts.Requests;

public class BaseGetRequest
{
    public int? PageIndex { get; set; } = 0;

    public int? PageSize { get; set; } = 10;
}
