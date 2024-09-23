using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NET8.Demo.RabbitMQ;

public static class RabbitMQModule
{
    public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureCap();
    }

    public static void ConfigureCap(this IServiceCollection services)
    {
        services.AddCap(options =>
        {
            options.UseSqlServer("Server=localhost;Database=Demo_GlobalAdmin;TrustServerCertificate=true;Trusted_Connection=True;")
                   .UseRabbitMQ(o =>
                   {
                       o.HostName = "localhost";
                       o.UserName = "guest";
                       o.Password = "guest";

                   })
                   .UseDashboard();
        });
    }

}
