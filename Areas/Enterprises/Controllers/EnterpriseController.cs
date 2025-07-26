using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Enterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN},{Roles.ENTERPRISE}")]
public class EnterpriseController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;

    public EnterpriseController(IEnterpriseService enterpriseService)
    {
        _enterpriseService = enterpriseService;
    }

    [AllowAnonymous]
    [HttpGet("get-many")]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _enterpriseService.GetManyAsync(pageNumber, limit, search);

            return Ok(new
            {
                totalEnterprises = result.totalEnterprises,
                enterprises = result.listEnterpriseDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOne(Guid id)
    {
        try
        {
            var enterprise = await _enterpriseService.GetOneAsync(id);

            return Ok(enterprise);
        }
        catch
        {
            throw;
        }
    }
    
    [HttpGet("get-my-many")]
    public async Task<IActionResult> GetMyMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _enterpriseService.GetMyManyAsync(User, pageNumber, limit, search);

            return Ok(new
            {
                totalEnterprises = result.totalEnterprises,
                enterprises = result.listEnterpriseDTOs
            });
        }
        catch
        {
            throw;
        }
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] EnterpriseDTO enterpriseDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _enterpriseService.CreateAsync(enterpriseDTO, User);

                return Ok("Tạo doanh nghiệp thành công");
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
    
    [HttpPut("update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EnterpriseDTO enterpriseDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _enterpriseService.UpdateAsync(id, enterpriseDTO, User);

                return Ok("Cập nhật doanh nghiệp thành công");
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
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _enterpriseService.DeleteAsync(id, User);

            return Ok("Xóa doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-ownership/{id}")]
    public async Task<IActionResult> AddOwnerShip(Guid id, [FromBody] string userId)
    {
        try
        {
            await _enterpriseService.AddOwnerShipAsync(id, userId, User);

            return Ok("Thêm sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }
    
    [HttpPut("giveup-ownership/{id}")]
    public async Task<IActionResult> GiveUpOwnership(Guid id)
    {
        try
        {
            await _enterpriseService.GiveUpOwnershipAsync(id, User);

            return Ok("Từ bỏ sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN}")]
    [HttpPut("delete-ownership/{id}")]
    public async Task<IActionResult> DeleteOwnership(Guid id, [FromBody] string userId)
    {
        try
        {
            await _enterpriseService.DeleteOwnershipAsync(id, userId);

            return Ok("Xóa sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }
}