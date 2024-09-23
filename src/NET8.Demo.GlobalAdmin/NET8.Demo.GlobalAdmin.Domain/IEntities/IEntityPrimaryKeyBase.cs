namespace NET8.Demo.GlobalAdmin.Domain.IEntities;

public interface IEntityPrimaryKeyBase<T>
{
    public T Id { get; set; }
}
