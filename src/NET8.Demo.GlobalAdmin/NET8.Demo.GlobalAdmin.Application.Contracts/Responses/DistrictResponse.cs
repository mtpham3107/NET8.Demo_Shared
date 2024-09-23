namespace NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

public class DistrictResponse
{
    public Guid Id { get; set; }

    public Guid ProvinceId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }
}
