using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Categories.Models;
using App.Areas.Comments.Models;
using App.Areas.Enterprises.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Products.Models;

[Table("Products")]
public class ProductModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Code { set; get; }

    public string Unit { set; get; }

    public string? Description { set; get; }

    public int? Quantity { set; get; }

    [Precision(18, 2)]
    public decimal? Discount { set; get; }

    public string? Status { set; get; }

    public string? Website { set; get; }

    [Precision(18, 2)]
    public decimal? Price { set; get; }

    [Required]
    public string UserId { set; get; }

    public Guid? CategoryId { set; get; }
    public Guid? EnterpriseId { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }

    [ForeignKey("CategoryId")]
    public CategoryModel? Category { set; get; }

    [ForeignKey("EnterpriseId")]
    public EnterpriseModel? Enterprise { set; get; }

    public List<CommentModel>? Comments { set; get; }
}