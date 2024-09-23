using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NET8.Demo.Redis.Mappers;
using NET8.Demo.Shared;
using System.Collections.Concurrent;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.Redis.Services.Implements;

public class DataSyncService<TRedisDto> : IDataSyncService<TRedisDto> where TRedisDto : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<DataSyncService<TRedisDto>> _logger;
    private readonly Type _dtoType;

    public DataSyncService(IServiceProvider serviceProvider, IMapper mapper, ILogger<DataSyncService<TRedisDto>> logger)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
        _dtoType = typeof(TRedisDto);
    }

    public async ValueTask<TRedisDto> GetByIdAsync(Guid id)
    {
        try
        {
            var (service, entityType) = GetService();
            return MapEntityToDto(await service.GetDataSyncToRedisAsync(id), entityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DataSyncService-GetByIdAsync-Exception: {id}", id);
            throw;
        }
    }

    public async ValueTask<IDictionary<string, TRedisDto>> GetListAsync(ICollection<Guid> ids)
    {
        try
        {
            var (service, entityType) = GetService();
            return await MapEntitiesToDtoAsync(await service.GetDataSyncToRedisAsync(ids), entityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DataSyncService-GetByIdAsync-Exception: {ids}", SerializeObject(ids));
            throw;
        }
    }

    public async ValueTask<IDictionary<string, TRedisDto>> GetListAsync()
    {
        try
        {
            var (service, entityType) = GetService();
            return await MapEntitiesToDtoAsync(await service.GetDataSyncToRedisAsync(), entityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DataSyncService-GetByIdAsync-Exception");
            throw;
        }
    }

    #region Private funtion
    public (dynamic Service, Type EntityType) GetService()
    {
        var entityService = DtoEntityMapper.GetEntityServiceMapping(_dtoType) ?? throw new BusinessException(ErrorCode.InternalServerError, $"No entity type found for DTO type {_dtoType}.");
        var service = _serviceProvider.GetRequiredService(entityService.ServiceType) ?? throw new BusinessException(ErrorCode.InternalServerError, $"No service found for entity type {entityService.ServiceType}.");
        return (service, entityService.EntityType);
    }

    public TRedisDto MapEntityToDto(object entity, Type entityType) => _mapper.Map(entity, entityType, _dtoType) as TRedisDto;

    public async ValueTask<IDictionary<string, TRedisDto>> MapEntitiesToDtoAsync(IEnumerable<object> entities, Type entityType)
    {
        var dbResults = new ConcurrentDictionary<string, TRedisDto>();

        var tasks = entities.Select(async entity => await Task.Run(() =>
        {
            var dto = _mapper.Map(entity, entityType, _dtoType) as TRedisDto;
            _ = dbResults.AddOrUpdate(GetEntityId(entity, entityType).ToString().ToLowerInvariant(), dto, (key, existingValue) => dto);
        }));

        await Task.WhenAll(tasks);
        return dbResults;
    }

    private static object GetEntityId(object entity, Type entityType) => entityType.GetProperty("Id").GetValue(entity);
    #endregion

}
