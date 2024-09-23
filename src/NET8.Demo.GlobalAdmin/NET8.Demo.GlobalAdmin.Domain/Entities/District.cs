using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class District : EntityBase
{
    public Guid ProvinceId { get; set; }

    [StringLength(200)]
    public string Code { get; set; }

    [StringLength(500)]
    public string Name { get; set; }

    [ForeignKey(nameof(ProvinceId))]
    public virtual Province Provinces { get; set; }

    public virtual ICollection<Ward> Wards { get; set; }
}
