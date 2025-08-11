
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Categories.Models;
using App.Areas.Factories.Models;
using App.Areas.Products.Models;
using App.Database;

namespace App.Areas.Enterprises.Models;

[Table("Enterprises")]
public class EnterpriseModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Name { set; get; }

    [Required]
    public string TaxCode { set; get; }

    [StringLength(13)]
    public string? GLNCode { set; get; }

    public string? Address { set; get; }

    public string? PhoneNumber { set; get; }

    public string? Email { set; get; }

    public string? Type { set; get; }

    public DateTime CreatedAt { set; get; }

    public DateTime? UpdatedAt { set; get; }

    public Guid? UpdatedUserId { set; get; }

    [ForeignKey("UpdatedUserId")]
    public AppUser? UpdatedUser { set; get; }

    public List<EnterpriseUserModel>? EnterpriseUsers { set; get; }

    public List<FactoryModel>? Factories { set; get; }
}