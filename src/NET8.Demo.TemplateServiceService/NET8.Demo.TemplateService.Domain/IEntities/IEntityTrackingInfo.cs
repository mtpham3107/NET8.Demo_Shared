namespace NET8.Demo.TemplateService.Domain.IEntities;

public interface IEntityTrackingInfo
{
    public Guid? CreatedBy { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
