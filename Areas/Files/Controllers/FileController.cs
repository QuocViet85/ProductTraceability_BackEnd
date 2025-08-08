using App.Areas.Files.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Files.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("entity")]
    public async Task<IActionResult> GetFilesByEntity(string entityType, string entityId, string? fileType = null, int limit = 0, bool descending = false)
    {
        try
        {
            var listFileDTOs = await _fileService.GetManyByEntityAsync(entityType, entityId, fileType, limit, descending);

            return Ok(listFileDTOs);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneById(Guid id)
    {
        try
        {
            var fileDTO = await _fileService.GetOneByIdAsync(id);

            return Ok(fileDTO);
        }
        catch
        {
            throw;
        }
    }
}