namespace NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

public class WardResponse
{
    public Guid Id { get; set; }

    public Guid DistrictId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }
}
