using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;

namespace Areas.Auth.DTO.Admin;

public class UserDTO
{
    public string? Id { set; get; }
    public string? UserName { set; get; }

    [DisplayName("Số điện thoại")]
    [Required(ErrorMessage = ErrorMessage.RequiredPhone)]
    [Phone(ErrorMessage = ErrorMessage.PhoneFormat)]
    public string PhoneNumber { set; get; }

    public string? Email { set; get; }
    public bool? IsActive { set; get; }

    [DisplayName("Mật khẩu")]
    [Required(ErrorMessage = ErrorMessage.RequiredPassword)]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài trong khoảng {0} đến {1} ký tự")]
    public string Password { set; get; }

    public string? Address { set; get; }

    public bool? PhoneNumberConfirmed { set; get; }

    [DisplayName("Vai trò")]
    public string Role { set; get; }
}