using StackExchange.Redis;

namespace NET8.Demo.Redis.ConnectionFactories;

public interface IRedisConnectionFactory
{
    public ConnectionMultiplexer Connection();

    public string ConnectionString();
}
