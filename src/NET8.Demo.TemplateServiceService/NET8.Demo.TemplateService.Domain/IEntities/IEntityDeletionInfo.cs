namespace NET8.Demo.TemplateService.Domain.IEntities;

public interface IEntityDeletionInfo
{
    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }
}