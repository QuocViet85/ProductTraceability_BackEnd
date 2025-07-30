using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Categories.Models;
using App.Areas.Comments.Models;
using App.Areas.Enterprises.Models;
using App.Areas.Factories.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Products.Models;

[Table("Products")]
public class ProductModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string TraceCode { set; get; }

    public string? Description { set; get; }

    public string? Website { set; get; }

    [Precision(18, 2)]
    public decimal? Price { set; get; }

    public Guid? CategoryId { set; get; }
    
    [ForeignKey("CategoryId")]
    public CategoryModel? Category { set; get; }

    [Required]
    public string CreatedUserId { set; get; }

    [ForeignKey("UserId")]
    public AppUser? CreatedUser { set; get; }

    public string? OwnerUserId { set; get; }

    [ForeignKey("OwnerUserId")]
    public AppUser? OwnerUser { set; get; }
    public string? ResponsibleUserId { set; get; }

    [ForeignKey("ResponsibleUserId")]
    public AppUser? ResponsibleUser { set; get; }
    public Guid? OwnerEnterpriseId { set; get; }

    [ForeignKey("OwnerEnterpriseId")]
    public EnterpriseModel? OwnerEnterprise { set; get; }
    public Guid? ProducerEnterpriseId { set; get; }

    [ForeignKey("ProducerEnterpriseId")]
    public EnterpriseModel? ProducerEnterprise { set; get; }
    public Guid? CarrierEnterpriseId { set; get; }
    
    [ForeignKey("CarrierEnterpriseId")]
    public EnterpriseModel? CarrierEnterprise { set; get; }
    public Guid? FactoryId { set; get; }

    [ForeignKey("FactoryId")]
    public FactoryModel? Factory { set; get; }
    public List<CommentModel>? Comments { set; get; }
}