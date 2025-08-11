using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.Auth.Models;

[Table("AspNetRefreshTokens")]
public class RefreshTokenModel
{
    [Key]
    public Guid Id { set; get; }

    public string Token { set; get; }

    public Guid UserId { set; get; }

    public DateTime ExpireTime { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }
}