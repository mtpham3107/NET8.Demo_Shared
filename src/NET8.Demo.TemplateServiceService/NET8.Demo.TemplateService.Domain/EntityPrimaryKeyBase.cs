using NET8.Demo.TemplateService.Domain.IEntities;

namespace NET8.Demo.TemplateService.Domain;

public class EntityPrimaryKeyBase<T> : IEntityPrimaryKeyBase<T>
{
    public T Id { get; set; }
}
