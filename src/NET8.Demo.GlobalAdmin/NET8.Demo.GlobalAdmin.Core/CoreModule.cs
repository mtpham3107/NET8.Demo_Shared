using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Core.Repositories;
using NET8.Demo.GlobalAdmin.Core.SeedDatas;
using NET8.Demo.GlobalAdmin.Core.UnitOfWorks;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;

namespace NET8.Demo.GlobalAdmin;

public static class CoreModule
{
    public static void ConfigureCore(this IServiceCollection services)
    {
        services.AddScoped<GlobalAdminDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<IUserRepository, UserRepository>();
    }

    public static async ValueTask ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GlobalAdminDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        if (!await HasSeededDataAsync(context))
        {
            await SeedDatabaseAsync(context);
        }
    }

    public static async ValueTask<bool> HasSeededDataAsync(GlobalAdminDbContext context)
    {
        return await context.Provinces.AnyAsync() ||
               await context.Districts.AnyAsync() ||
               await context.Wards.AnyAsync();
    }

    public static async Task SeedDatabaseAsync(GlobalAdminDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        await DataSeeder.SeedAdministrativeUnitsAsync(context);
    }
}
