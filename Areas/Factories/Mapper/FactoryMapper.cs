using App.Areas.Factories.DTO;
using App.Areas.Factories.Models;

namespace App.Areas.Factories.Mapper;

public static class FactoryMapper
{
    public static FactoryDTO ModelToDto(FactoryModel factory)
    {
        return new FactoryDTO()
        {
            Id = factory.Id,
            Name = factory.Name,
            Address = factory.Address,
            ContactInfo = factory.ContactInfo,
            FactoryCode = factory.FactoryCode,
            CreatedAt = factory.CreatedAt,
            UpdatedAt = factory.UpdatedAt
        };
    }
    public static FactoryModel DtoToModel(FactoryDTO factoryDTO, FactoryModel factoryUpdate = null)
    {
        FactoryModel factory;
        if (factoryUpdate == null)
        {
            factory = new FactoryModel();
        }
        else
        {
            factory = factoryUpdate;
        }

        factory.Name = factoryDTO.Name;
        factory.Address = factoryDTO.Address;
        factory.ContactInfo = factoryDTO.ContactInfo;

        return factory;
    }
}