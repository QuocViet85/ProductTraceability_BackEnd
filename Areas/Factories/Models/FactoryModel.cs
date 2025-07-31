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

    public string? Address { set; get; }

    public string? ContactInfo { set; get; }

    public DateTime CreatedAt { set; get; }
    public string? CreatedUserId { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser? CreatedUser { set; get; }
    public string? OwnerIndividualEnterpriseId { set; get; }

    [ForeignKey("OwnerIndividualEnterpriseId")]
    public IndividualEnterpriseModel? OwnerIndividualEnterprise { set; get; }
    public Guid? EnterpriseId { set; get; }

    [ForeignKey("EnterpriseId")]
    public EnterpriseModel? Enterprise { set; get; }
}