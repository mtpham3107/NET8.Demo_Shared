using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class Address : EntityBase
{
    public Guid ProvinceId { get; set; }

    public Guid DistrictId { get; set; }

    public Guid WardId { get; set; }

    [StringLength(4000)]
    public string AddressLine { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
