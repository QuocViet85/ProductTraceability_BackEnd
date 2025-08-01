using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Areas.Enterprises.DTO;
using App.Areas.IndividualEnterprises.DTO;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.Factories.DTO;

public class FactoryDTO
{
    public Guid? Id { set; get; }

    [DisplayName("Tên nhà máy")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }
    public string? FactoryCode { set; get; }
    public string? Address { set; get; }
    public string? ContactInfo { set; get; }
    public bool IndividualEnterpriseOwner { set; get; }
    public Guid? EnterpriseId { set; get; }
    public DateTime? CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public UserDTO? CreatedUser { set; get; }
    public UserDTO? UpdatedUser { set; get; }
    public IndividualEnterpriseDTO? IndividualEnterprise { set; get; }
    public EnterpriseDTO? Enterprise { set; get; }
}