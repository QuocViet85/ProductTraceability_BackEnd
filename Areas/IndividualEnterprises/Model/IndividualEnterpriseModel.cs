using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.IndividualEnterprises.Model;

[Table("IndividualEnterprises")]
public class IndividualEnterpriseModel
{
    [Key]
    public Guid OwnerUserId { set; get; }

    [Required]
    public string Name { set; get; }

    [Required]
    public string IndividualEnterpriseCode { set; get; }

    public string? TaxCode { set; get; }

    public string? GLNCode { set; get; }

    public string? Address { set; get; }

    public string? PhoneNumber { set; get; }

    public string? Email { set; get; }

    public string? Type { set; get; }

    public DateTime CreatedAt { set; get; }

    [ForeignKey("OwnerUserId")]
    public AppUser OwnerUser { set; get; }

    public DateTime? UpdatedAt { set; get; }

    public Guid? UpdatedUserId { set; get; }

    [ForeignKey("UpdatedUserId")]
    public AppUser? UpdatedUser { set; get; }
}