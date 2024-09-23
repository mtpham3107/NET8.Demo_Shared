using NET8.Demo.GlobalAdmin.Domain.IEntities;

namespace NET8.Demo.GlobalAdmin.Domain;

public class EntityPrimaryKeyBase<T> : IEntityPrimaryKeyBase<T>
{
    public T Id { get; set; }
}
