using System.Security.Claims;
using Areas.Auth.DTO.Admin;

namespace App.Areas.Auth.Services;

public interface IAuthAdminService
{
    public Task Create(UserDTO userDTO);

    public Task<(int totalUsers, List<UserDTO> listUsers)> GetMany(int pageNumber, int limit, string search);
    public Task<UserDTO> GetOne(string id);
    public Task Update(ClaimsPrincipal userNowFromJwt, string id, UserDTO userDTO);
    public Task Delete(ClaimsPrincipal userNowFromJwt, string id);
}