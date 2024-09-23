namespace NET8.Demo.Redis.Mappers;

public static class DtoEntityMapper
{
    private static Dictionary<Type, ServiceMapping> _dtoToEntityMapping = [];

    static DtoEntityMapper() => InitializeMappings();

    public static ServiceMapping GetEntityServiceMapping(Type dtoType) => _dtoToEntityMapping.TryGetValue(dtoType, out var mapping) ? mapping : null;

    private static void RegisterMappings(Dictionary<Type, ServiceMapping> newMappings) => _dtoToEntityMapping = _dtoToEntityMapping.Union(newMappings).ToDictionary();

    private static void InitializeMappings()
    {
        RegisterMappings(GlobalAdminMapper.Mappings);
    }
}

