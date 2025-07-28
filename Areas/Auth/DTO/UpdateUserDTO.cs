using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;

namespace App.Areas.Auth.DTO;

public class UpdateUserDTO
{
    [DisplayName("Tên")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }
    
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string? Email { set; get; }
    public string? Address { set; get; }
}