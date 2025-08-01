using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.Enterprises.DTO;

public class EnterpriseDTO
{
    public Guid? Id { set; get; }

    [DisplayName("Tên công ty")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }

    [DisplayName("Mã số thuế")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string TaxCode { set; get; }

    [DisplayName("Mã GLN")]
    [StringLength(13, ErrorMessage = "{0} chỉ có thể có 13 kí tự")]
    public string? GLNCode { set; get; }
    public string? Address { set; get; }
    public string? PhoneNumber { set; get; }
    public string? Email { set; get; }
    public string? Type { set; get; }
    public List<EnterpriseUserDTO>? Owners { set; get; }
    public DateTime? CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public EnterpriseUserDTO? UpdatedUser { set; get; }
}

public class EnterpriseUserDTO : UserDTO
{
    public bool CreatedBy { set; get; }
}