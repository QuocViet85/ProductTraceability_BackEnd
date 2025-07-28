

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;
namespace App.Areas.Auth.DTO;
public class LoginDTO
{
    [DisplayName("Số điện thoại")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string PhoneNumber { set; get; }

    [DisplayName("Mật khẩu")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Password { set; get; }
}