using System.Security.Claims;
using Areas.Auth.DTO.Admin;

namespace App.Areas.Auth.Services;

public interface IAuthAdminService
{
    public Task Create(UserDTO userDTO);

    public Task<(int totalUsers, List<UserDTO> listUsers)> GetAll(int pageNumber, int limit);
    public Task<UserDTO> GetOne(string id);
    public Task Update(string id, UserDTO userDTO, ClaimsPrincipal userNowFromJwt);
    public Task Delete(string id, ClaimsPrincipal userNowFromJwt);
}