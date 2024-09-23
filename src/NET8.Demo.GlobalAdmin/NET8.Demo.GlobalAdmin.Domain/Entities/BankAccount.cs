using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class BankAccount : EntityBase
{
    [StringLength(200)]
    public string AccountNumber { get; set; }

    [StringLength(500)]
    public string HolderName { get; set; }

    [StringLength(200)]
    public string BankName { get; set; }

    [StringLength(200)]
    public string BranchName { get; set; }
}
