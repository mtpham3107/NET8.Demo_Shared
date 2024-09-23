using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

public class AddressResponse
{
    public Guid Id { get; set; }

    public Guid ProvinceId { get; set; }

    public string ProvinceName { get; set; }

    public Guid DistrictId { get; set; }

    public string DistrictName { get; set; }

    public Guid WardId { get; set; }

    public string WardName { get; set; }

    [StringLength(4000)]
    public string AddressLine { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
