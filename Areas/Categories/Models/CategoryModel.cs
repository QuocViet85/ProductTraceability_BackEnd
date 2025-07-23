using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Enterprises.Models;
using App.Areas.Products.Models;
using Database;

namespace App.Areas.Categories.Models;

[Table("Categories")]
public class CategoryModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Name { set; get; }

    public string? Description { set; get; }

    public bool IsDefault { set; get; }

    public Guid? EnterpriseId { set; get; }
    public string UserId { set; get; }


    [ForeignKey("EnterpriseId")]
    public EnterpriseModel Enterprise { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }

    public List<ProductModel> Products { set; get; }
}