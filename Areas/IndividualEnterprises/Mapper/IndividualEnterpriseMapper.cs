using App.Areas.IndividualEnterprises.DTO;
using App.Areas.IndividualEnterprises.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace App.Areas.IndividualEnterprises.Mapper;

public static class IndividualEnterpriseMapper
{
    public static IndividualEnterpriseDTO ModelToDto(IndividualEnterpriseModel individualEnterprise)
    {
        return new IndividualEnterpriseDTO()
        {
            OwnerUserId = individualEnterprise.OwnerUserId,
            Name = individualEnterprise.Name,
            TaxCode = individualEnterprise.TaxCode,
            Address = individualEnterprise.Address,
            PhoneNumber = individualEnterprise.PhoneNumber,
            Email = individualEnterprise.Email,
            Type = individualEnterprise.Type,
            CreatedAt = individualEnterprise.CreatedAt
        };
    }

    public static IndividualEnterpriseModel DtoToModel(IndividualEnterpriseDTO individualEnterpriseDTO, IndividualEnterpriseModel individualEnterpriseUpdate = null)
    {
        IndividualEnterpriseModel individualEnterprise = new IndividualEnterpriseModel();

        if (individualEnterpriseUpdate == null)
        {
            individualEnterprise = new IndividualEnterpriseModel();
        }
        else
        {
            individualEnterprise = individualEnterpriseUpdate;
        }

        individualEnterprise.Name = individualEnterpriseDTO.Name;
        individualEnterprise.TaxCode = individualEnterpriseDTO.TaxCode;
        individualEnterprise.Address = individualEnterpriseDTO.Address;
        individualEnterprise.PhoneNumber = individualEnterpriseDTO.PhoneNumber;
        individualEnterprise.Email = individualEnterpriseDTO.Email;
        individualEnterprise.Type = individualEnterpriseDTO.Type;

        return individualEnterprise;
    }
}
