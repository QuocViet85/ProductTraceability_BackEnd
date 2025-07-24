using System.Security.Claims;
using App.Areas.Enterprises.DTO;

namespace App.Areas.Enterprises.Services;

public interface IEnterpriseService
{
    public Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMany(int pageNumber, int limit, string search);
    public Task<EnterpriseDTO> GetOne(Guid Id);
    public Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMyMany(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search);
    public Task Create(ClaimsPrincipal userNowFromJwt, EnterpriseDTO enterpriseDTO);
    public Task Update(ClaimsPrincipal userNowFromJwt, Guid Id, EnterpriseDTO enterpriseDTO);
    public Task Delete(ClaimsPrincipal userNowFromJwt, Guid Id);

    public Task AddOwnerShip(ClaimsPrincipal userNowFromJwt, Guid Id);
    public Task GiveUpOwnership(ClaimsPrincipal userNowFromJwt, Guid Id);
    public Task DeleteOwnership(ClaimsPrincipal userNowFromJwt, Guid Id);
}