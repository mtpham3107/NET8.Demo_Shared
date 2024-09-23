using Microsoft.AspNetCore.Identity;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
        Id = Guid.NewGuid();
    }
}
