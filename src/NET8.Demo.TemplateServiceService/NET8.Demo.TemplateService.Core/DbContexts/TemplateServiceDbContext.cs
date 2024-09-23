using Microsoft.EntityFrameworkCore;

namespace NET8.Demo.TemplateService.Core.DbContexts;

public class TemplateServiceDbContext : DbContext, IDisposable
{
    public TemplateServiceDbContext(DbContextOptions<TemplateServiceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
