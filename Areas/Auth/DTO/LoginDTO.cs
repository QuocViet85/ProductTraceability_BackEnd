

using System.ComponentModel.DataAnnotations;
using App.Messages;
namespace App.Areas.Auth.DTO;
public class LoginDTO
{
    [Required(ErrorMessage = ErrorMessage.RequiredPhone)]
    public string PhoneNumber { set; get; }

    [Required(ErrorMessage = ErrorMessage.RequiredPhone)]
    public string Password { set; get; }
}