using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NET8.Demo.TemplateService.Core.DbContexts;

public class TemplateServiceDbContextFactory : IDesignTimeDbContextFactory<TemplateServiceDbContext>
{
    public TemplateServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<TemplateServiceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"), n => n.MigrationsAssembly("NET8.Demo.TemplateService.Core"));

        return new TemplateServiceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../NET8.Demo.TemplateService/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
