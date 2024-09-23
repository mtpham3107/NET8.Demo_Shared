using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Handler;
using NET8.Demo.GlobalAdmin.Application.IServices;
using NET8.Demo.GlobalAdmin.Application.Services;
using NET8.Demo.GlobalAdmin.Application.SignalRHubs;

namespace NET8.Demo.GlobalAdmin;

public static class ApplicationModule
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        services.AddScoped(typeof(IService<>), typeof(Service<>));
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<NotificationHandler>();
        services.AddScoped<ILocationService, LocationService>();
    }
}