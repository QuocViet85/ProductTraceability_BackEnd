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
            await _authService.RegisterAsync(registerDTO);
            return Ok("Đăng kí thành công");
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

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(id);

            return Ok(user);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("user-name/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserNameById(Guid id)
    {
        try
        {
            var user = await _authService.GetUserNameByIdAsync(id);

            return Ok(user);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyUser()
    {
        try
        {
            var user = await _authService.GetMyUserAsync(User);

            return Ok(user);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("access-token")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        try
        {
            await _authService.LogoutAsync(refreshToken);

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

    [HttpPut]
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

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            await _authService.ChangePasswordAsync(User, changePasswordDTO);

            return Ok("Đổi mật khẩu thành công");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> SetAvatar(IFormFile file)
    {
        try
        {
            await _authService.SetAvatarAsync(User, file);

            return Ok("Đặt ảnh đại diện thành công");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authService.DeleteAvatarAsync(User);

                return Ok("Xóa ảnh đại diện thành công");
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

    [HttpPut("cover-photo")]
    public async Task<IActionResult> SetCoverPhoto(IFormFile file)
    {
        try
        {
            await _authService.SetCoverPhotoAsync(User, file);

            return Ok("Đặt ảnh bìa thành công");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("cover-photo")]
    public async Task<IActionResult> DeleteCoverPhoto()
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authService.DeleteCoverPhotoAsync(User);

                return Ok("Xóa ảnh bìa thành công");
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

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        try
        {
            if (ModelState.IsValid)
            {
                var permissions = await _authService.GetPermissionsAsync(User);

                return Ok(permissions);
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

    [HttpGet("kiem-tra-theo-doi/{id}")]
    public async Task<IActionResult> TheoDoiHoacHuyTheoDoi(Guid id)
    {
        try
        {
            bool daTheoDoi = await _authService.KiemTraTheoDoiAsync(User, id);

            return Ok(daTheoDoi);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("theo-doi/{userId}")]
    public async Task<IActionResult> KiemTraTheoDoi(Guid userId)
    {
        try
        {
            await _authService.TheoDoiHoacHuyTheoDoiUserAsync(User, userId);

            return Ok();
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("so-luong-theo-doi/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LaySoLuongTheoDoi(Guid id)
    {
        try
        {
            int soLuong = await _authService.LaySoTheoDoiUserAsync(id);

            return Ok(soLuong);
        }
        catch
        {
            throw;
        }
    }
}