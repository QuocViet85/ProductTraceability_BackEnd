using System.ComponentModel.DataAnnotations;

namespace App.Areas.Auth.DTO;

public class UpdateUserDTO
{
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string? Email { set; get; }
    public string? UserName { set; get; }
    public string? Address { set; get; }
}