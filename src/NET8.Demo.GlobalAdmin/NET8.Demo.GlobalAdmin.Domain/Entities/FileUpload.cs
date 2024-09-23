using System.ComponentModel.DataAnnotations;

namespace NET8.Demo.GlobalAdmin.Domain.Entities;

public class FileUpload : EntityBase
{
    [StringLength(500)]
    public string FileName { get; set; }

    [StringLength(2000)]
    public string FileUrl { get; set; }

    public long? FileSize { get; set; }

    [StringLength(10)]
    public string FileExtension { get; set; }
}
