using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Areas.Auth.AuthorizationType;
using App.Messages;

namespace App.Areas.Auth.DTO;

public class RegisterDTO
{
    [DisplayName("Tên")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }

    [DisplayName("Số điện thoại")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    [Phone(ErrorMessage = ErrorMessage.PhoneFormat)]
    public string PhoneNumber { set; get; }

    [DisplayName("Mật khẩu")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài trong khoảng {0} đến {1} ký tự")]
    public string Password { set; get; }

    [DisplayName("Vai trò")]
    [Required(ErrorMessage = "Vui lòng chọn vai trò")]
    public string Role { set; get; } = Roles.CUSTOMER;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string? Email { set; get; }

    public string? Address { set; get; }
}