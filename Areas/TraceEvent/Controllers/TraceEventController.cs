using App.Areas.Auth.AuthorizationType;
using App.Areas.TraceEvents.DTO;
using App.Areas.TraceEvents.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.TraceEvents.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class TraceEventController : ControllerBase
{
    private readonly ITraceEventService _traceEventService;

    public TraceEventController(ITraceEventService traceEventService)
    {
        _traceEventService = traceEventService;
    }

    [HttpGet("get-many-by-batch/{batchId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetManyByBatch(Guid batchId, int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _traceEventService.GetManyByBatchAsync(batchId, pageNumber, limit, search);

            return Ok(new
            {
                totalTraceEvents = result.totalItems,
                traceEvents = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one-by-id/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneById(Guid id)
    {
        try
        {
            var traceEventDTO = await _traceEventService.GetOneByIdAsync(id);

            return Ok(traceEventDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one-by-traceEventCode/{traceEventCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByTraceEventCode(string traceEventCode)
    {
        try
        {
            var traceEventDTO = await _traceEventService.GetOneByTraceEventCodeAsync(traceEventCode);

            return Ok(traceEventDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] TraceEventDTO traceEventDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _traceEventService.CreateAsync(traceEventDTO, User);

                return Ok("Tạo sự kiện truy xuất thành công");
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
    public async Task<IActionResult> Update(Guid id, [FromBody] TraceEventDTO traceEventDTO)
    {
        try
        {
            await _traceEventService.UpdateAsync(id, traceEventDTO, User);

            return Ok("Cập nhật sự kiện truy xuất thành công");
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
            await _traceEventService.DeleteAsync(id, User);

            return Ok("Xóa sự kiện truy xuất thành công");
        }
        catch
        {
            throw;
        }
    }
}