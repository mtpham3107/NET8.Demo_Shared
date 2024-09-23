using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class Province : EntityBase
{
    [StringLength(200)]
    public string Code { get; set; }

    [StringLength(500)]
    public string Name { get; set; }

    public virtual ICollection<District> Districts { get; set; }
}
