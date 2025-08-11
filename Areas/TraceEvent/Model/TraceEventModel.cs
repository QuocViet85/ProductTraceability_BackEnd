using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Batches.Models;
using App.Database;

namespace App.Areas.TraceEvents.Models;

[Table("TraceEvents")]
public class TraceEventModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public Guid BatchId { set; get; }

    [Required]
    public string Name { set; get; }

    [Required]
    public string TraceEventCode { set; get; }
    public string? Description { set; get; }
    public string? Location { set; get; }
    public DateTime TimeStamp { set; get; }
    public Guid CreatedUserId { set; get; }
    public Guid? UpdatedUserId { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }

    [ForeignKey("BatchId")]
    public BatchModel Batch { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser CreatedUser { set; get; }

    [ForeignKey("UpdatedUserId")]
    public AppUser? UpdatedUser { set; get; }
}