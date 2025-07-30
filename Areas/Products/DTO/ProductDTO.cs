using System.ComponentModel.DataAnnotations;
using App.Areas.Categories.DTO;
using App.Areas.Enterprises.DTO;
using App.Areas.Factories.DTO;
using Areas.Auth.DTO;

namespace App.Areas.Products.DTO;

public class ProductDTO
{
    public Guid Id { set; get; }

    public string? TraceCode { set; get; }

    public string? Description { set; get; }

    public string? Website { set; get; }

    public decimal? Price { set; get; }

    public DateTime? CreatedAt { set; get; }

    public DateTime? UpdatedAt { set; get; }

    public CategoryDTO? Category { set; get; }

    public UserDTO? CreatedUser { set; get; }

    public UserDTO? OwnerUser { set; get; }

    public UserDTO? ResponsibleUser { set; get; }

    public EnterpriseDTO? OwnerEnterprise { set; get; }

    public EnterpriseDTO? ProducerEnterprise { set; get; }

    public EnterpriseDTO? CarrierEnterprise { set; get; }

    public FactoryDTO? Factory { set; get; }
}