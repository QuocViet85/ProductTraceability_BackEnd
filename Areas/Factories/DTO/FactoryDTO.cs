using System.ComponentModel.DataAnnotations;
using App.Areas.Enterprises.DTO;
using Areas.Auth.DTO;

namespace App.Areas.Factories.DTO;

public class FactoryDTO
{
    public Guid? Id { set; get; }

    [Required]
    public string Name { set; get; }
    public string? Address { set; get; }
    public string? ContactInfo { set; get; }
    public DateTime CreatedAt { set; get; }
    public UserDTO? CreatedUser { set; get; }
    public UserDTO? OwnerUser { set; get; }
    public EnterpriseDTO? Enterprise { set; get; }
}