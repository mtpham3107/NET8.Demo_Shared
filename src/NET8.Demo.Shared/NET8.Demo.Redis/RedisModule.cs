
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.Redis.ConnectionFactories;
using NET8.Demo.Redis.ConnectionFactories.Implements;
using NET8.Demo.Redis.Services;
using NET8.Demo.Redis.Services.Implements;

namespace NET8.Demo.Redis;

public static class RedisModule
{
    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(options =>
        {
            options.ConnectionString = "localhost:6379,defaultDatabase=2";
        });
        services.AddAutoMapper(typeof(GlobalAdminMapper));
        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        services.AddTransient(typeof(IRedisService<>), typeof(RedisService<>));
        services.AddTransient(typeof(IDataSyncService<>), typeof(DataSyncService<>));
    }
}
