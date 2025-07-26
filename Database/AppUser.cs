using System.ComponentModel.DataAnnotations;
using App.Areas.Batches.Models;
using App.Areas.Categories.Models;
using App.Areas.Enterprises.Models;
using App.Areas.Products.Models;
using Microsoft.AspNetCore.Identity;

namespace App.Database;

public class AppUser : IdentityUser
{
    [Required]
    [Range(0, 500)]
    public override string PhoneNumber { set; get; }

    [Required]
    public string Name { set; get; }

    [Range(0, 255)]
    public string? Organization { set; get; }

    public DateTime CreatedAt { set; get; }

    public string? Address { set; get; }

    public bool IsActive { set; get; }
}