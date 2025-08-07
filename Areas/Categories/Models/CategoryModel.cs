using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Enterprises.Models;
using App.Areas.Products.Models;
using App.Database;

namespace App.Areas.Categories.Models;

[Table("Categories")]
public class CategoryModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Name { set; get; }

    [Required]
    public bool IsParent { set; get; }

    public string? Description { set; get; }

    public Guid? ParentCategoryId { set; get; }

    public string CreatedUserId { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser CreatedUser { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public string? UpdatedUserId { set; get; }
    public AppUser? UpdatedUser { set; get; }
    public CategoryModel? ParentCategory { set; get; }
    public List<CategoryModel>? ChildCategories { set; get; }
}