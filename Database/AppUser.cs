using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace App.Database;

public class AppUser : IdentityUser<Guid>
{
    [Required]
    [Range(0, 500)]
    public override string PhoneNumber { set; get; }

    [Required]
    public string Name { set; get; }

    public DateTime CreatedAt { set; get; }

    public string? Address { set; get; }

    public bool IsActive { set; get; }
}