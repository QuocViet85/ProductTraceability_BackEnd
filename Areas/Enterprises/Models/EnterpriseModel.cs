using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Categories.Models;
using App.Areas.Factories.Models;
using App.Areas.Products.Models;

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

    public string? Address { set; get; }

    public string? PhoneNumber { set; get; }

    public string? Email { set; get; }

    public string? Type { set; get; }

    public DateTime CreatedAt { set; get; }

    public List<CategoryModel>? Categories { set; get; }

    public List<EnterpriseUserModel>? EnterpriseUsers { set; get; }

    public List<ProductModel> Products { set; get; }

    public List<FactoryModel>? Factories { set; get; }
}