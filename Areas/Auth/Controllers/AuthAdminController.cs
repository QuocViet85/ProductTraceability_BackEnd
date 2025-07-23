
using App.Areas.Auth.Services;
using App.Messages;
using Areas.Auth.DTO.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.ADMIN)]
public class AuthAdminController : ControllerBase
{
    private readonly IAuthAdminService _authAdminService;

    public AuthAdminController(IAuthAdminService authAdminService)
    {
        _authAdminService = authAdminService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authAdminService.Create(userDTO);

                return Ok("Tạo user thành công");
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

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(int pageNumber, int limit)
    {
        try
        {
            var result = await _authAdminService.GetAll(pageNumber, limit);

            return Ok(new
            {
                totalUsers = result.totalUsers,
                listUsers = result.listUsers
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
        try
        {
            var user = await _authAdminService.GetOne(id);

            return Ok(user);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserDTO userDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authAdminService.Update(id, userDTO, User);

                return Ok("Cập nhật user thành công");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        catch
        {
            throw;
        }
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] string id)
    {
        try
        {
            await _authAdminService.Delete(id, User);

            return Ok("Xóa user thành công");
        }
        catch
        {
            throw;
        }
    }
}