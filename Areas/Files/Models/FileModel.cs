using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.Files.Models;

[Table("Files")]
public class FileModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string FileName { set; get; }

    [Required]
    public string FileType { set; get; }

    public long? Size { set; get; }

    [Required]
    public string EntityType { set; get; }

    [Required]
    public string EntityId { set; get; }

    public string? CreatedUserId { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser? CreatedUser { set; get; }
}