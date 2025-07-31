using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.IndividualEnterprises.DTO;

public class IndividualEnterpriseDTO
{
    [Key]
    public string? OwnerUserId { set; get; }

    [DisplayName("Tên hộ kinh doanh cá nhân")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }
    public string? TaxCode { set; get; }

    public string? GLNCode { set; get; }

    public string? Address { set; get; }

    public string? PhoneNumber { set; get; }

    public string? Email { set; get; }

    public string? Type { set; get; }

    public DateTime? CreatedAt { set; get; }

    public UserDTO? OwnUser { set; get; }
}