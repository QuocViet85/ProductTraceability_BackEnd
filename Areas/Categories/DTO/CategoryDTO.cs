using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.Categories.DTO;

public class CategoryDTO
{
    public Guid? Id { set; get; }

    [DisplayName("danh mục sản phẩm")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }
    public string? Description { set; get; }
    public UserDTO? User { set; get; }
    public DateTime? CreatedAt { set; get; }
}