using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Batches.Models;
using App.Database;

namespace App.Areas.TraceEvent.Models;

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

    public DateTime TimeStamp { set; get; }

    public string CreatedUserId { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("BatchId")]
    public BatchModel Batch { set; get; }

    [ForeignKey("UserId")]
    public AppUser CreatedUser { set; get; }
}