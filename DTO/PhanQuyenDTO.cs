using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;

namespace App.Areas.DTO;

public class PhanQuyenDTO
{   
    [DisplayName("Người dùng")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public Guid UserId { set; get; }
    public bool Admin { set; get; }
    public bool Them { set; get; }
    public bool Sua { set; get; }
    public bool Xoa { set; get; }
}