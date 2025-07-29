using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Enterprises.Models;
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
    public string? OwnerUserId { set; get; }

    [ForeignKey("OwnerUserId")]
    public AppUser? OwnerUser { set; get; }
    public Guid? EnterpriseId { set; get; }

    [ForeignKey("EnterpriseId")]
    public EnterpriseModel? Enterprise { set; get; }
}