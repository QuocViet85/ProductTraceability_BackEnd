using App.Areas.Auth.AuthorizationType;
using App.Areas.DanhMuc.Models;
using App.Areas.DanhMuc.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DanhMuc.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}")]
public class DanhMucController : ControllerBase
{
    private readonly IDanhMucService _danhMucService;

    public DanhMucController(IDanhMucService danhMucService)
    {
        _danhMucService = danhMucService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieu()
    {
        try
        {
            var result = await _danhMucService.LayNhieuAsync(0, 0, "", false);

            return Ok(new
            {
                totalCategories = result.totalItems,
                categories = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayBangId(Guid id)
    {
        try
        {
            var danhMuc = await _danhMucService.LayMotBangIdAsync(id);

            return Ok(danhMuc);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("name/{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayMotBangTen(string name)
    {
        try
        {
            var danhMuc = await _danhMucService.LayMotBangTenAsync(name);

            return Ok(danhMuc);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Them([FromBody] DanhMucModel danhMuc)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _danhMucService.ThemAsync(danhMuc, User);

                return Ok("Tạo danh mục sản phẩm thành công");
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
    public async Task<IActionResult> Sua(Guid id, [FromBody] DanhMucModel danhMuc)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _danhMucService.SuaAsync(id, danhMuc, User);

                return Ok("Cập nhật danh mục sản phẩm thành công");
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
    public async Task<IActionResult> Xoa(Guid id)
    {
        try
        {
            await _danhMucService.XoaAsync(id, User);

            return Ok("Xóa danh mục sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }
}