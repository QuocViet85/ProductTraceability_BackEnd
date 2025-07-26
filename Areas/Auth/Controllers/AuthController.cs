using App.Areas.Auth.DTO;
using App.Areas.Auth.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authService.RegisterAsync(registerDTO);

                return Ok("Đăng kí thành công");
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var token = await _authService.LoginAsync(loginDTO);

                return Ok(new
                {
                    accessToken = token.accessToken,
                    refreshToken = token.refreshToken
                });
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
            
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpGet("/api/user/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneUser(string id)
    {
        try
        {
            var user = await _authService.GetOneUserAsync(id);

            return Ok(user);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("access-token")]
    public async Task<IActionResult> GetAccessToken([FromBody] string refreshToken)
    {
        try
        {
            var accessToken = await _authService.GetAccessTokenAsync(refreshToken);

            return Ok(accessToken);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        try
        {
            await _authService.LogoutAsync(User, refreshToken);

            return Ok("Đăng xuất thành công");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("logoutAll")]
    public async Task<IActionResult> LogoutAllDevices()
    {
        try
        {
            await _authService.LogoutAllDevicesAsync(User);

            return Ok("Đăng xuất thành công");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateUserDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authService.UpdateAsync(User, updateUserDTO);

                return Ok("Cập nhật thành công");
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authService.ChangePasswordAsync(User, changePasswordDTO);

                return Ok("Đổi mật khẩu thành công");
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}