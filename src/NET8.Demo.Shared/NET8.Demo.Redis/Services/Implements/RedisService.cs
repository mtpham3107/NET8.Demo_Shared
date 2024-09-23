using Elastic.Apm.StackExchange.Redis;
using Microsoft.Extensions.Logging;
using NET8.Demo.Redis.ConnectionFactories;
using NET8.Demo.Shared;
using StackExchange.Redis;
using System.Collections.Concurrent;
using static Newtonsoft.Json.JsonConvert;
using static System.Text.Encoding;

namespace NET8.Demo.Redis.Services.Implements;

public class RedisService<TRedisDto> : IRedisService<TRedisDto> where TRedisDto : class
{
    private readonly IDataSyncService<TRedisDto> _dataSyncService;
    private readonly ILogger<RedisService<TRedisDto>> _logger;
    private readonly IRedisConnectionFactory _connectionFactory;
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisService(
        ILogger<RedisService<TRedisDto>> logger,
        IRedisConnectionFactory connectionFactory,
        IDataSyncService<TRedisDto> dataSyncService)
    {
        _dataSyncService = dataSyncService;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _connectionMultiplexer = _connectionFactory.Connection();
        _connectionMultiplexer.UseElasticApm();
        _database = _connectionMultiplexer.GetDatabase();
    }

    public async ValueTask<TRedisDto> GetAsync(string group, string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(key))
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            var normalizedGroup = group.ToLowerInvariant();
            var normalizedKey = key.ToString().ToLowerInvariant();

            var redisResult = await _database.HashGetAsync(normalizedGroup, normalizedKey);
            if (redisResult.HasValue)
            {
                return DeserializeObject<TRedisDto>(UTF8.GetString(redisResult));
            }

            var dbResult = await _dataSyncService.GetByIdAsync(Guid.Parse(normalizedKey));
            if (dbResult == null)
            {
                return default;
            }

            _ = Task.Run(() => SetAsync(group, key, dbResult));

            return dbResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-GetAsync-Exception: {Group} - {Key}", group, key);

            throw;
        }
    }

    public async ValueTask<IDictionary<string, TRedisDto>> GetListAsync(string group, ICollection<string> keys)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group) || keys.Count == 0)
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            var normalizedGroup = group.ToLowerInvariant();
            var normalizedKeys = keys.Select(k => k.ToLowerInvariant()).ToList();
            var redisResults = new ConcurrentDictionary<string, TRedisDto>();

            var tasks = normalizedKeys.Select(async key =>
            {
                var value = await _database.HashGetAsync(normalizedGroup, key);
                if (value.HasValue)
                {
                    redisResults[key] = DeserializeObject<TRedisDto>(value);
                }
            });

            await Task.WhenAll(tasks);

            var missingKeys = normalizedKeys
                .Where(key => !redisResults.ContainsKey(key))
                .Select(Guid.Parse)
                .ToList();

            if (missingKeys.Count != 0)
            {
                var dbResults = await _dataSyncService.GetListAsync(missingKeys);

                tasks = dbResults.Select(async x => await Task.Run(() => redisResults.AddOrUpdate(x.Key, x.Value, (key, existingValue) => x.Value)));

                await Task.WhenAll(tasks);

                if (dbResults.Any())
                {
                    _ = Task.Run(() => SetBulkAsync(normalizedGroup, dbResults));
                }
            }

            return redisResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-GetListAsync-Exception: {Group} - {Keys}", group, string.Join(", ", keys));

            throw;
        }
    }

    public async ValueTask<IDictionary<string, TRedisDto>> GetListAsync(string group)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            var normalizedGroup = group.ToLowerInvariant();
            var redisResults = new ConcurrentDictionary<string, TRedisDto>();
            var redisHashEntries = await _database.HashGetAllAsync(normalizedGroup);

            var tasks = redisHashEntries
                .Where(entry => entry.Name.HasValue && entry.Value.HasValue)
                .Select(entry => Task.Run(() =>
                {
                    var key = entry.Name.ToString();
                    var redisValue = entry.Value;

                    if (redisValue.HasValue)
                    {
                        var value = DeserializeObject<TRedisDto>(redisValue);
                        redisResults[key] = value;
                    }
                }));

            await Task.WhenAll(tasks);

            if (!redisResults.IsEmpty)
            {
                return redisResults;
            }

            var dbResults = await _dataSyncService.GetListAsync();
            if (dbResults.Any())
            {
                _ = Task.Run(() => SetBulkAsync(normalizedGroup, dbResults));
            }

            return dbResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-GetAllAsync-Exception: {Group}", group);

            throw;
        }
    }

    public async ValueTask<bool> SetAsync(string group, string key, TRedisDto value)
    {
        var jsonVal = SerializeObject(value);

        try
        {
            if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(key) || value == null)
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            _ = await _database.HashSetAsync(group.ToLowerInvariant(), key.ToString().ToLowerInvariant(), jsonVal);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-SetAsync-Exception: {Group} - {Key} - {Value}", group, key, jsonVal);

            throw;
        }
    }

    public async ValueTask<bool> SetBulkAsync(string group, IDictionary<string, TRedisDto> fields)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group) || !fields.Any())
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            await _database.HashSetAsync(group.ToLowerInvariant(), fields.Select(p => new HashEntry(p.Key.ToString().ToLowerInvariant(), SerializeObject(p.Value))).ToArray());

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetBulk-RedisService-Exception: {Group} - {Fields}", group, SerializeObject(fields));

            throw;
        }
    }

    public async ValueTask<bool> DeleteAsync(string group, string key)
    {
        try
        {
            return string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(key)
                ? throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data")
                : await _database.HashDeleteAsync(group.ToLowerInvariant(), key.ToLowerInvariant());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-DeleteAsync-Exception: {Group} - {Key}", group, key);

            throw;
        }
    }

    public async ValueTask<bool> DeleteBulkAsync(string group, ICollection<string> keys)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group) || keys.Count == 0)
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            var rslt = true;
            var semSlim = new SemaphoreSlim(1);

            await Task.WhenAll(keys.Where(string.IsNullOrWhiteSpace).Select(async k =>
            {
                await semSlim.WaitAsync();

                try
                {
                    rslt = rslt && await _database.HashDeleteAsync(group.ToLowerInvariant(), k.ToLowerInvariant());
                }
                finally
                {
                    _ = semSlim.Release();
                }
            }));

            return rslt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-DeleteBulkAsync-Exception: {Group} - {Keys}", group, string.Join(", ", keys));

            throw;
        }
    }

    public async ValueTask<bool> DeleteAsync(string group)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new BusinessException(ErrorCode.InternalServerError, "Invalid input data");
            }

            var normalizedGroup = group.ToLowerInvariant();
            var redisResults = new ConcurrentDictionary<string, TRedisDto>();
            var redisHashEntries = await _database.HashGetAllAsync(normalizedGroup);

            var ids = redisHashEntries
                .Where(entry => entry.Name.HasValue && entry.Value.HasValue)
                .Select(entry => entry.Name.ToString())
                .ToArray();

            return ids.Length == 0 || await DeleteBulkAsync(group, ids);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RedisService-DeleteAllAsync-Exception: {Group}", group);

            throw;
        }
    }
}
