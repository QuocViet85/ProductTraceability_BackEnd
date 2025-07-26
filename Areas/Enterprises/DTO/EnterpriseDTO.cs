using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Database;

namespace App.Areas.Enterprises.DTO;

public class EnterpriseDTO
{
    public Guid? Id { set; get; }

    [DisplayName("Tên công ty")]
    [Required(ErrorMessage = "Vui lòng nhập {0}")]
    public string Name { set; get; }

    [DisplayName("Mã số thuế")]
    [Required(ErrorMessage = "Vui lòng nhập {0}")]
    public string TaxCode { set; get; }

    [DisplayName("Mã GLN")]
    [StringLength(13, ErrorMessage = "{0} chỉ có thể có 13 kí tự")]
    public string? GLNCode { set; get; }
    public string? Address { set; get; }
    public string? PhoneNumber { set; get; }
    public string? Email { set; get; }
    public string? Type { set; get; }
    public List<EnterpriseUserDTO>? Owners { set; get; }
}

public class EnterpriseUserDTO
{
    public string Id { set; get; }
    public string UserName { set; get; }
    public string Role { set; get; }
    public bool CreatedBy { set; get; }
}