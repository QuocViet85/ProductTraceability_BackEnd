using System.Security.Claims;
using App.Areas.Factories.DTO;
using App.Services;

namespace App.Areas.Factories.Services;

public interface IFactoryService : IBaseService<FactoryDTO>
{
    public Task AddEnterpriseToFactoryAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteEnterpriseInFactoryAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddIndividualEnterpriseToFactoryAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteIndividualEnterpriseInFactoryAsync(Guid id, ClaimsPrincipal userNowFromJwt);

}