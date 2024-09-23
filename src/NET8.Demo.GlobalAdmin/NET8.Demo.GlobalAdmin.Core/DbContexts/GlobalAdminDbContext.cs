using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.Shared;

namespace NET8.Demo.GlobalAdmin.Core.DbContexts;

public class GlobalAdminDbContext(DbContextOptions options) : IdentityDbContext<User, Role, Guid>(options), IDisposable
{
    public DbSet<FileUpload> FileUploads { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Address>()
           .Property(a => a.Latitude)
           .HasColumnType("decimal(9,6)");

        builder.Entity<Address>()
            .Property(a => a.Longitude)
            .HasColumnType("decimal(9,6)");

        AddDefaultUser(builder);
    }

    private static void AddDefaultUser(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher<User>();
        var userId = Guid.NewGuid();
        var role = new List<Role>()
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = SharedConstant.Role.Admin,
                NormalizedName = SharedConstant.Role.Admin.ToUpper()
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = SharedConstant.Role.Supplier,
                NormalizedName = SharedConstant.Role.Supplier.ToUpper()
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = SharedConstant.Role.Affiliate,
                NormalizedName = SharedConstant.Role.Affiliate.ToUpper()
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = SharedConstant.Role.Customer,
                NormalizedName = SharedConstant.Role.Customer.ToUpper()
            },
        };

        var user = new User
        {
            Id = userId,
            LastName = "Admin",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "123456"),
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            IsActive = true,
        };

        var userRole = new IdentityUserRole<Guid>
        {
            UserId = userId,
            RoleId = role[0].Id,
        };

        _ = modelBuilder.Entity<Role>().HasData(role);
        _ = modelBuilder.Entity<User>().HasData(user);
        _ = modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(userRole);
    }
}
