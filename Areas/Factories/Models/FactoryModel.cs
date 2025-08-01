using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Enterprises.Models;
using App.Areas.IndividualEnterprises.Model;
using App.Database;

namespace App.Areas.Factories.Models;

[Table("Factories")]
public class FactoryModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Name { set; get; }

    [Required]
    public string FactoryCode { set; get; }

    public string? Address { set; get; }

    public string? ContactInfo { set; get; }

    public DateTime CreatedAt { set; get; }
    public string? CreatedUserId { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser? CreatedUser { set; get; }
    public string? IndividualEnterpriseId { set; get; }

    [ForeignKey("IndividualEnterpriseId")]
    public IndividualEnterpriseModel? IndividualEnterprise { set; get; }
    public Guid? EnterpriseId { set; get; }

    [ForeignKey("EnterpriseId")]
    public EnterpriseModel? Enterprise { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public string? UpdatedUserId { set; get; }

    [ForeignKey("UpdatedUserId")]
    public AppUser? UpdatedUser { set; get; }
}