using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.Files.Models;

[Table("Files")]
public class FileModel
{
    [Key]
    public Guid Id { set; get; }

    public string? FileName { set; get; }

    public string? FileType { set; get; }

    public int? Size { set; get; }

    public string? EntityType { set; get; }

    public Guid EntityId { set; get; }

    public string UserId { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }
}