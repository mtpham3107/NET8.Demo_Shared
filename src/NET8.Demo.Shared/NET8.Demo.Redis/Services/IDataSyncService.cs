namespace NET8.Demo.Redis.Services;

public interface IDataSyncService<TRedisDto> where TRedisDto : class
{
    ValueTask<TRedisDto> GetByIdAsync(Guid id);

    ValueTask<IDictionary<string, TRedisDto>> GetListAsync(ICollection<Guid> ids);

    ValueTask<IDictionary<string, TRedisDto>> GetListAsync();
}
