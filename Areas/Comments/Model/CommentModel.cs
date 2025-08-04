using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.Products.Models;
using App.Database;

namespace App.Areas.Comments.Models;

[Table("Comments")]
public class CommentModel
{
    [Key]
    public Guid Id { set; get; }

    [Required]
    public string Content { set; get; }

    [Required]
    public string CreatedUserId { set; get; }

    [Required]
    public Guid ProductId { set; get; }
    public DateTime CreatedAt { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }

    [ForeignKey("CreatedUserId")]
    public AppUser CreatedUser { set; get; }
}