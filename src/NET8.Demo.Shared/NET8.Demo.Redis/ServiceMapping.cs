namespace NET8.Demo.Redis;

public class ServiceMapping
{
    public Type EntityType { get; set; }

    public Type ServiceType { get; set; }

    public ServiceMapping(Type entityType, Type serviceType)
    {
        EntityType = entityType;
        ServiceType = serviceType;
    }
}
