using App.Areas.Auth.AuthorizationType;
using App.Areas.Factories.DTO;
using App.Areas.Factories.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Factories.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class FactoryController : ControllerBase
{
    private readonly IFactoryService _factoryService;

    public FactoryController(IFactoryService factoryService)
    {
        _factoryService = factoryService;
    }

    [HttpGet("get-many")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _factoryService.GetManyAsync(pageNumber, limit, search);

            return Ok(new
            {
                totalCategories = result.totalItems,
                categories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one/{factoryCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOne(string factoryCode)
    {
        try
        {
            var factoryDTO = await _factoryService.GetOneByFactoryCodeAsync(factoryCode);

            return Ok(factoryDTO);
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
            var result = await _factoryService.GetMyManyAsync(User, pageNumber, limit, search);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                categories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] FactoryDTO factoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _factoryService.CreateAsync(factoryDTO, User);

                return Ok("Tạo nhà máy thành công");
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

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] FactoryDTO factoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _factoryService.UpdateAsync(id, factoryDTO, User);

                return Ok("Cập nhật nhà máy thành công");
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
            await _factoryService.DeleteAsync(id, User);

            return Ok("Xóa nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-enterprise/{id}")]
    public async Task<IActionResult> AddEnterpriseToFactory(Guid id, [FromBody] Guid enterpriseId)
    {
        try
        {
            await _factoryService.AddEnterpriseToFactoryAsync(id, enterpriseId, User);

            return Ok("Thêm doanh nghiệp vào nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-enterprise/{id}")]
    public async Task<IActionResult> DeleteEnterpriseInFactory(Guid id)
    {
        try
        {
            await _factoryService.DeleteEnterpriseInFactoryAsync(id, User);

            return Ok("Xóa doanh nghiệp sở hữu nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-ownership/{id}")]
    public async Task<IActionResult> AddIndividualEnterpriseToFactory(Guid id, [FromBody] string userId)
    {
        try
        {
            await _factoryService.AddIndividualEnterpriseToFactoryAsync(id, userId, User);

            return Ok("Thêm sở hữu cá nhân vào nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-ownership/{id}")]
    public async Task<IActionResult> DeleteIndividualEnterpriseInFactory(Guid id)
    {
        try
        {
            await _factoryService.DeleteIndividualEnterpriseInFactoryAsync(id, User);

            return Ok("Xóa sở hữu cá nhân của nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }
}