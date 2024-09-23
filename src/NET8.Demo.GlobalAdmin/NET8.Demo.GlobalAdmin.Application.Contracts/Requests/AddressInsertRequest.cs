namespace NET8.Demo.GlobalAdmin.Application.Contracts.Requests;

public class AddressInsertRequest
{
    public Guid ProvinceId { get; set; }

    public Guid DistrictId { get; set; }

    public Guid WardId { get; set; }

    public string AddressLine { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
