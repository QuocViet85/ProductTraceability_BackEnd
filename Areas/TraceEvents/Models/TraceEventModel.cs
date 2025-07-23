using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database;

namespace App.Areas.Batches.Models;

[Table("TraceEvents")]
public class TraceEventModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public Guid BatchId { set; get; }

    [Required]
    public string EventType { set; get; }

    public string? Description { set; get; }

    public string? Location { set; get; }

    public string UserId { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("BatchId")]
    public BatchModel Batch { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }
}