namespace NET8.Demo.GlobalAdmin.Domain.IEntities;

public interface IEntityDeletionInfo
{
    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }
}