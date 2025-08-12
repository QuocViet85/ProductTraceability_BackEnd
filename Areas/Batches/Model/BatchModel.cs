using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.NhaMay.Models;
using App.Areas.SanPham.Models;
using App.Database;

namespace App.Areas.Batches.Models;

[Table("Batches")]
public class BatchModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string BatchCode { set; get; }

    [Required]
    public Guid ProductId { set; get; }

    public string? Name { set; get; }
    public DateTime? ManufactureDate { set; get; }
    public DateTime? ExpireDate { set; get; }
    public int? Quantity { set; get; }
    public string? Status { set; get; }
    public Guid? FactoryId { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public Guid CreatedUserId { set; get; }
    public Guid? UpdatedUserId { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser CreatedUser { set; get; }

    [ForeignKey("UpdatedUserId")]
    public AppUser? UpdatedUser { set; get; }

    [ForeignKey("FactoryId")]
    public FactoryModel? Factory { set; get; }


}