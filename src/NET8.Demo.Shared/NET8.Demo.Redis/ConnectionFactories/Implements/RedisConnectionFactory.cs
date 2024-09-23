using Microsoft.Extensions.Options;
using StackExchange.Redis;
using static StackExchange.Redis.ConnectionMultiplexer;

namespace NET8.Demo.Redis.ConnectionFactories.Implements;

public class RedisConnectionFactory : IRedisConnectionFactory
{
    private readonly RedisSettings _option;
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisConnectionFactory(IOptions<RedisSettings> options)
    {
        _option = options.Value;
        _connection = new Lazy<ConnectionMultiplexer>(() => Connect(_option.ConnectionString));
    }

    public ConnectionMultiplexer Connection() => _connection.Value;

    public string ConnectionString() => (_option.ConnectionString ?? ",").Split(',')[0];
}
