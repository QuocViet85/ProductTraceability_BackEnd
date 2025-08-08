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

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _factoryService.GetManyAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                totalFactories = result.totalItems,
                factories = result.listDTOs
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
            var factoryDTO = await _factoryService.GetOneByIdAsync(id);

            return Ok(factoryDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("factory-code/{factoryCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByFactoryCode(string factoryCode)
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

    [HttpGet("me")]
    public async Task<IActionResult> GetMyMany(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _factoryService.GetMyManyAsync(User, pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                factories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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

    [HttpPost("enterprise/{id}")]
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

    [HttpDelete("enterprise/{id}")]
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

    [HttpPost("individual-enterprise/{id}")]
    public async Task<IActionResult> AddIndividualEnterpriseToFactory(Guid id, [FromBody] string userId)
    {
        try
        {
            await _factoryService.AddIndividualEnterpriseToFactoryAsync(id, userId, User);

            return Ok("Thêm hộ kinh doanh cá nhân sở hữu nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("individual-enterprise/{id}")]
    public async Task<IActionResult> DeleteIndividualEnterpriseInFactory(Guid id)
    {
        try
        {
            await _factoryService.DeleteIndividualEnterpriseInFactoryAsync(id, User);

            return Ok("Xóa hộ kinh doanh cá nhân sở hữu nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }
}