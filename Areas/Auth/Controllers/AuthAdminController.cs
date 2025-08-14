using App.Areas.Auth.AuthorizationData;
using App.Areas.Auth.Services;
using App.Messages;
using Areas.Auth.DTO;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authAdminService.CreateAsync(userDTO);

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

    [HttpGet]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string search = null)
    {
        try
        {
            var result = await _authAdminService.GetManyAsync(pageNumber, limit, search);

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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO userDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _authAdminService.UpdateAsync(id, userDTO, User);

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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] Guid id)
    {
        try
        {
            await _authAdminService.DeleteAsync(id, User);

            return Ok("Xóa user thành công");
        }
        catch
        {
            throw;
        }
    }
}