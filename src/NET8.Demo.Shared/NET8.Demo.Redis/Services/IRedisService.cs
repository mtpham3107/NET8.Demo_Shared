namespace NET8.Demo.Redis.Services;

public interface IRedisService<TRedisDto> where TRedisDto : class
{
    public ValueTask<TRedisDto> GetAsync(string group, string key);

    public ValueTask<IDictionary<string, TRedisDto>> GetListAsync(string group, ICollection<string> keys);

    public ValueTask<IDictionary<string, TRedisDto>> GetListAsync(string group);

    public ValueTask<bool> SetAsync(string group, string key, TRedisDto value);

    public ValueTask<bool> SetBulkAsync(string group, IDictionary<string, TRedisDto> fields);

    public ValueTask<bool> DeleteAsync(string group, string key);

    public ValueTask<bool> DeleteBulkAsync(string group, ICollection<string> keys);

    ValueTask<bool> DeleteAsync(string group);
}
