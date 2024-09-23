using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.TemplateService.Application.Contracts.IServices;
using NET8.Demo.TemplateService.Application.Services;

namespace NET8.Demo.TemplateService;

public static class ApplicationModule
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IService<>), typeof(Service<>));
    }
}