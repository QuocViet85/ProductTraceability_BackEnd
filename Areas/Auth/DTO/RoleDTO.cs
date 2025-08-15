using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;

namespace App.Areas.Auth.DTO;

public class RoleDTO
{
    [DisplayName("Người dùng")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public Guid UserId { set; get; }
    public bool Admin { set; get; }
    public bool Doanh_Nghiep { set; get; }
    public bool Khach_Hang { set; get; }
}