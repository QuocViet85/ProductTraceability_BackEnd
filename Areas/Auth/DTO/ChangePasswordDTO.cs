using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;

namespace App.Areas.Auth.DTO;

public class ChangePasswordDTO
{
    [DisplayName("Mật khẩu cũ")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string OldPassword { set; get; }


    [DisplayName("Mật khẩu mới")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài trong khoảng {0} đến {1} ký tự")]
    public string NewPassword { set; get; }
}

