using System.Security.Claims;
using App.Areas.Auth.DTO;
using App.Database;
using Areas.Auth.DTO;

namespace App.Areas.Auth.Services;

public interface IAuthService
{
    public Task RegisterAsync(RegisterDTO registerDTO);

    public Task<(string accessToken, string refreshToken)> LoginAsync(LoginDTO loginDTO);

    public Task<UserDTO> GetOneUserAsync(string id);

    public Task<string> GetAccessTokenAsync(string refreshToken);

    public Task LogoutAsync(ClaimsPrincipal userNowFromJwt, string refreshToken);

    public Task LogoutAllDevicesAsync(ClaimsPrincipal userNowFromJwt);

    public Task UpdateAsync(ClaimsPrincipal userNowFromJwt, UpdateUserDTO userUpdateDTO);

    public Task ChangePasswordAsync(ClaimsPrincipal userNowFromJwt, ChangePasswordDTO changePasswordDTO);
}