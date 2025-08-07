using App.Areas.Auth.AuthorizationType;
using App.Areas.Batches.DTO;
using App.Areas.Batches.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Batches.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class BatchController : ControllerBase
{
    private readonly IBatchService _batchService;

    public BatchController(IBatchService batchService)
    {
        _batchService = batchService;
    }

    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetManyByProduct(Guid productId, int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _batchService.GetManyByProductAsync(productId, pageNumber, limit, search);

            return Ok(new
            {
                totalBatches = result.totalItems,
                Batches = result.listDTOs
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
            var batchDTO = await _batchService.GetOneByIdAsync(id);

            return Ok(batchDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByBatchCode(string batchCode)
    {
        try
        {
            var batchDTO = await _batchService.GetOneByBatchCodeAsync(batchCode);

            return Ok(batchDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BatchDTO batchDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _batchService.CreateAsync(batchDTO, User);

                return Ok("Tạo lô hàng thành công");
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
    public async Task<IActionResult> Update(Guid id, [FromBody] BatchDTO batchDTO)
    {
        try
        {
            await _batchService.UpdateAsync(id, batchDTO, User);

            return Ok("Cập nhật lô hàng thành công");
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
            await _batchService.DeleteAsync(id, User);

            return Ok("Xóa lô hàng thành công");
        }
        catch
        {
            throw;
        }
    }
}