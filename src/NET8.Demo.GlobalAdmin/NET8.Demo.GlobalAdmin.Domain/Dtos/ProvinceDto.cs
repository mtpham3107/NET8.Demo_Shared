namespace NET8.Demo.GlobalAdmin.Domain.Dtos;

public class ProvinceDto
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }
}
