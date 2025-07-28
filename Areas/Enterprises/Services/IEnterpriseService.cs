using System.Security.Claims;
using App.Areas.Enterprises.DTO;
using App.Services;

namespace App.Areas.Enterprises.Services;

public interface IEnterpriseService : IBaseService<EnterpriseDTO>
{
    public Task AddOwnerShipAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt);
    public Task GiveUpOwnershipAsync(Guid id, ClaimsPrincipal userNowFromJwt);
    public Task DeleteOwnershipAsync(Guid id, string userId);
}