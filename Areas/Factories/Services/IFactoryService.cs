using System.Security.Claims;
using App.Areas.Factories.DTO;
using App.Services;

namespace App.Areas.Factories.Services;

public interface IFactoryService : IBaseService<FactoryDTO>
{
    public Task AddEnterpriseToFactory(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task AddOwnerShipToFactory(Guid id, string userId, ClaimsPrincipal userNowFromJwt);
}