using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using App.Areas.Factories.Models;
using App.Database;

namespace App.Areas.Batches.Models;

[Table("Batches")]
public class BatchModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Code { set; get; }

    public DateTime? ManufactureDate { set; get; }

    public DateTime? ExpireDate { set; get; }

    public int Quantity { set; get; }

    public string? Status { set; get; }

    public Guid? FactoryId { set; get; }

    public string UserId { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("FactoryId")]
    public FactoryModel? Factory { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }

    public List<TraceEventModel>? TraceEvents { set; get; }
}