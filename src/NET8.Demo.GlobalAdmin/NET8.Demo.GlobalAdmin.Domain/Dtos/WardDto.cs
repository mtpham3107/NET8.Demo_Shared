namespace NET8.Demo.GlobalAdmin.Domain.Dtos;

public class WardDto
{
    public Guid Id { get; set; }

    public Guid ProvinceId { get; set; }

    public string ProvinceCode { get; set; }

    public Guid DistrictId { get; set; }

    public string DistrictCode { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }
}
