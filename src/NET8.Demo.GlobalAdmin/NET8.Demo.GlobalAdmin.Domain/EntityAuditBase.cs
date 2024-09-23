using NET8.Demo.GlobalAdmin.Domain.IEntities;

namespace NET8.Demo.GlobalAdmin.Domain;

public class EntityAuditBase<T> : IEntityAuditBase<T>
{
    public T Id { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool IsActive { get; set; } = true;
}
