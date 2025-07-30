using App.Areas.IndividualEnterprises.DTO;
using App.Areas.IndividualEnterprises.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.IndividualEnterprises.Controllers;

public class IndividualEnterpiseController : ControllerBase
{
    private readonly IIndividualEnterpiseService _individualEnterpriseService;

    public IndividualEnterpiseController(IIndividualEnterpiseService individualEnterpiseService)
    {
        _individualEnterpriseService = individualEnterpiseService;
    }

    [AllowAnonymous]
    [HttpGet("get-many")]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _individualEnterpriseService.GetManyAsync(pageNumber, limit, search);

            return Ok(new
            {
                totalIndividualEnterprises = result.totalItems,
                individualEnterprises = result.listDTOs
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
            var individualEnterpriseDTO = await _individualEnterpriseService.GetOneAsync(id);

            return Ok(individualEnterpriseDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-my-one")]
    public async Task<IActionResult> GetMyOne()
    {
        try
        {
            var individualEnterpriseDTO = await _individualEnterpriseService.GetMyOneAsync(User);

            return Ok(individualEnterpriseDTO);
        }
        catch
        {
            throw;
        }
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] IndividualEnterpriseDTO individualEnterpriseDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _individualEnterpriseService.CreateAsync(individualEnterpriseDTO, User);

                return Ok("Tạo hộ kinh doanh cá nhân thành công");
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
    public async Task<IActionResult> Update(Guid id, [FromBody] IndividualEnterpriseDTO individualEnterpriseDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _individualEnterpriseService.UpdateAsync(id, individualEnterpriseDTO, User);

                return Ok("Cập nhật hộ kinh doanh cá nhân thành công");
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
            await _individualEnterpriseService.DeleteAsync(id, User);

            return Ok("Xóa hộ kinh doanh cá nhân thành công");
        }
        catch
        {
            throw;
        }
    }
}