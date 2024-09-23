using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NET8.Demo.GlobalAdmin.Core.DbContexts;

public class GlobalAdminDbContextFactory : IDesignTimeDbContextFactory<GlobalAdminDbContext>
{
    public GlobalAdminDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<GlobalAdminDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"), n => n.MigrationsAssembly("NET8.Demo.GlobalAdmin.Core"));

        return new GlobalAdminDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../NET8.Demo.GlobalAdmin/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
