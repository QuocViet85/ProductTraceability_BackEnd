using System.Security.Claims;
using Areas.Auth.DTO;

namespace App.Areas.Auth.Services;

public interface IAuthAdminService
{
    public Task CreateAsync(UserDTO userDTO);

    public Task<(int totalUsers, List<UserDTO> listUsers)> GetManyAsync(int pageNumber, int limit, string search);
    
    public Task UpdateAsync(Guid id, UserDTO userDTO, ClaimsPrincipal userNowFromJwt);
    public Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt);
}