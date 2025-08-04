using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.Comments.Models;

public class CommentDTO
{
    public Guid? Id { set; get; }

    [DisplayName("Nội dung bình luận")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Content { set; get; }

    [DisplayName("Sản phẩm")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public Guid ProductId { set; get; }
    public DateTime? CreatedAt { set; get; }
    public UserDTO? CreatedUser { set; get; }
}

