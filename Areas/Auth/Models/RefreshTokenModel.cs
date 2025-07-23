using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database;

namespace App.Areas.Auth.Models;

[Table("RefreshTokens")]
public class RefreshTokenModel
{
    [Key]
    public Guid Id { set; get; }

    public string Token { set; get; }

    public string UserId { set; get; }

    public DateTime ExpireTime { set; get; }

    [ForeignKey("UserId")]
    public AppUser User { set; get; }
}