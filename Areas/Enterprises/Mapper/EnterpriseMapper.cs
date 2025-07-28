using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Models;

namespace App.Areas.Enterprises.Mapper;

public static class EnterpriseMapper
{
       public static EnterpriseDTO ModelToDto(EnterpriseModel enterprise)
    {
        var enterpriseDTO = new EnterpriseDTO()
        {
            Id = enterprise.Id,
            Name = enterprise.Name,
            TaxCode = enterprise.TaxCode,
            GLNCode = enterprise.GLNCode,
            Address = enterprise.Address,
            PhoneNumber = enterprise.PhoneNumber,
            Email = enterprise.Email,
            Type = enterprise.Type,
            CreatedAt = enterprise.CreatedAt,
            UpdatedAt = enterprise.UpdatedAt,
        };
        
        return enterpriseDTO;
    }

    public static EnterpriseModel DtoToModel(EnterpriseDTO enterpriseDTO, EnterpriseModel enterpriseUpdate = null)
    {
        EnterpriseModel enterprise;
        if (enterpriseUpdate == null)
        {
            enterprise = new EnterpriseModel();
        }
        else
        {
            enterprise = enterpriseUpdate;
        }

        enterprise.Name = enterpriseDTO.Name;
        enterprise.TaxCode = enterpriseDTO.TaxCode;
        enterprise.GLNCode = enterpriseDTO.GLNCode;
        enterprise.Address = enterpriseDTO.Address;
        enterprise.PhoneNumber = enterpriseDTO.PhoneNumber;
        enterprise.Email = enterpriseDTO.Email;
        enterprise.Type = enterpriseDTO.Type;

        return enterprise;
    }
}