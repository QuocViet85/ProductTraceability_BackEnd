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

    [HttpGet("tai-nguyen")]
    public async Task<IActionResult> LayNhieuBangTaiNguyen(string kieuTaiNguyen, Guid taiNguyenId, string? kieuFile = null, int limit = 0, bool descending = false)
    {
        try
        {
            var listFileDTOs = await _fileService.LayNhieuBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId, kieuFile, limit, descending);

            return Ok(listFileDTOs);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> LayMotBangId(Guid id)
    {
        try
        {
            var fileDTO = await _fileService.LayMotBangIdAsync(id);

            return Ok(fileDTO);
        }
        catch
        {
            throw;
        }
    }
}