using System.Security.Claims;
using App.Areas.IndividualEnterprises.DTO;
using App.Services;

namespace App.Areas.IndividualEnterprises.Services;

public interface IIndividualEnterpiseService : IBaseService<IndividualEnterpriseDTO>
{
    public Task<IndividualEnterpriseDTO> GetMyOneAsync(ClaimsPrincipal userNowFromJwt);
    public Task<IndividualEnterpriseDTO> GetOneByIndividualEnterpriseCodeAsync(string individualEnterpiseCode);
}