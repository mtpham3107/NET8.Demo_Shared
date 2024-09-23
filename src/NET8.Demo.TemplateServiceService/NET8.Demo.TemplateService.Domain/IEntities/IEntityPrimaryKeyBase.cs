namespace NET8.Demo.TemplateService.Domain.IEntities;

public interface IEntityPrimaryKeyBase<T>
{
    public T Id { get; set; }
}
