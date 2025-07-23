using System.Security.Claims;
using App.Areas.Auth.DTO;
using Database;

namespace App.Areas.Auth.Services;

public interface IAuthService
{
    public Task Register(RegisterDTO registerDTO);

    public Task<(string accessToken, string refreshToken)> Login(LoginDTO loginDTO);

    public Task<string> GetAccessToken(string refreshToken);

    public Task Logout(ClaimsPrincipal userNowFromJwt, string refreshToken);

    public Task LogoutAllDevices(ClaimsPrincipal userNowFromJwt);

    public Task Update(ClaimsPrincipal userNowFromJwt, UpdateUserDTO userUpdateDTO);

    public Task ChangePassword(ClaimsPrincipal userNowFromJwt, ChangePasswordDTO changePasswordDTO);
}