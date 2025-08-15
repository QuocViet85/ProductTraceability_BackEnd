using App.Areas.Auth.AuthorizationData;
using App.Areas.LoSanPham.Models;
using App.Areas.LoSanPham.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.LoSanPham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.DOANH_NGHIEP}")]
public class LoSanPhamController : ControllerBase
{
    private readonly ILoSanPhamService _loSanPhamService;

    public LoSanPhamController(ILoSanPhamService loSanPhamService)
    {
        _loSanPhamService = loSanPhamService;
    }

    [HttpGet("san-pham/{sp_id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangSanPham(Guid sp_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _loSanPhamService.LayNhieuBangSanPhamAsync(sp_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listLoSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayMotBangId(Guid id)
    {
        try
        {
            var loSanPham = await _loSanPhamService.LayMotBangIdAsync(id);

            return Ok(loSanPham);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LayMotBangMaLoSanPham(string lsp_MaLSP)
    {
        try
        {
            var loSanPham = await _loSanPhamService.LayMotBangMaLoSanPhamAsync(lsp_MaLSP);

            return Ok(loSanPham);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost()]
    public async Task<IActionResult> Them([FromBody] LoSanPhamModel loSanPham)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _loSanPhamService.ThemAsync(loSanPham, User);

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
    public async Task<IActionResult> Sua(Guid id, [FromBody] LoSanPhamModel loSanPham)
    {
        try
        {
            await _loSanPhamService.SuaAsync(id, loSanPham, User);

            return Ok("Cập nhật lô hàng thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Xoa(Guid id)
    {
        try
        {
            await _loSanPhamService.XoaAsync(id, User);

            return Ok("Xóa lô hàng thành công");
        }
        catch
        {
            throw;
        }
    }
}