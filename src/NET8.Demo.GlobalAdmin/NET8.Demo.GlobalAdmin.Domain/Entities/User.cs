using Microsoft.AspNetCore.Identity;
using NET8.Demo.GlobalAdmin.Domain.IEntities;
using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class User : IdentityUser<Guid>, IEntityTrackingInfo, IEntityDeletionInfo
{
    [StringLength(200)]
    public string FirstName { get; set; }

    [StringLength(200)]
    public string LastName { get; set; }

    [StringLength(2000)]
    public string AvatarUrl { get; set; }

    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();

    public Guid? CreatedBy { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

}
