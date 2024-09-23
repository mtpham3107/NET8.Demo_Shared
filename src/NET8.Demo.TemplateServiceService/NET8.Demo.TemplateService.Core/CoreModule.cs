using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.TemplateService.Core.DbContexts;
using NET8.Demo.TemplateService.Core.Repositories;
using NET8.Demo.TemplateService.Core.UnitOfWorks;
using NET8.Demo.TemplateService.Domain.IRepositories;
using NET8.Demo.TemplateService.Domain.IUnitOfWorks;

namespace NET8.Demo.TemplateService;

public static class CoreModule
{
    public static void ConfigureCore(this IServiceCollection services)
    {
        services.AddScoped<TemplateServiceDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TemplateServiceDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}
