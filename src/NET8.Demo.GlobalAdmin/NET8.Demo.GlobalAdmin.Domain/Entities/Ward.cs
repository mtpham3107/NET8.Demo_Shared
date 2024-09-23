using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class Ward : EntityBase
{
    public Guid DistrictId { get; set; }

    [StringLength(200)]
    public string Code { get; set; }

    [StringLength(4000)]
    public string Name { get; set; }

    [ForeignKey(nameof(DistrictId))]
    public virtual District District { get; set; }
}
