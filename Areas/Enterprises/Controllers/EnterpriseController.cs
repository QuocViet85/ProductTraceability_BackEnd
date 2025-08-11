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
    [HttpGet]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _enterpriseService.GetManyAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                enterprises = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneById(Guid id)
    {
        try
        {
            var enterpriseDTO = await _enterpriseService.GetOneByIdAsync(id);

            return Ok(enterpriseDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("tax-code/{taxCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByTaxCode(string taxCode)
    {
        try
        {
            var enterpriseDTO = await _enterpriseService.GetOneByTaxCode(taxCode);

            return Ok(enterpriseDTO);
        }
        catch
        {
            throw;
        }
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetMyMany(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _enterpriseService.GetMyManyAsync(User, pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                enterprises = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }
    
    [HttpPost]
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
    
    [HttpPut("{id}")]
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
    
    [HttpDelete("{id}")]
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

    [HttpPost("ownership/{id}")]
    public async Task<IActionResult> AddOwnerShip(Guid id, [FromBody] Guid userId)
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
    
    [HttpDelete("ownership/me/{id}")]
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
    [HttpDelete("ownership/{id}")]
    public async Task<IActionResult> DeleteOwnership(Guid id, [FromBody] Guid userId)
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