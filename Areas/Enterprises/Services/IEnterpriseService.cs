using System.Security.Claims;
using App.Areas.Enterprises.DTO;

namespace App.Areas.Enterprises.Services;

public interface IEnterpriseService
{
    public Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetManyAsync(int pageNumber, int limit, string search);
    public Task<EnterpriseDTO> GetOneAsync(Guid id);
    public Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search);
    public Task CreateAsync(EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt);
    public Task UpdateAsync(Guid id, EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt);
    public Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddOwnerShipAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt);
    public Task GiveUpOwnershipAsync(Guid id, ClaimsPrincipal userNowFromJwt);
    public Task DeleteOwnershipAsync(Guid id, string userId);
}