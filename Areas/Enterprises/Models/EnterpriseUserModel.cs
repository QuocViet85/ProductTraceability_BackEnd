using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.Enterprises.Models;

[Table("EnterpriseUser")]
public class EnterpriseUserModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public Guid EnterpriseId { set; get; }

    [Required]
    public string UserId { set; get; }

    [ForeignKey("EnterpriseId")]
    public EnterpriseModel Enterprise { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }

    public bool CreatedBy { set; get; }
}