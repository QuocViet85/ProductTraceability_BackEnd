using App.Areas.Auth.AuthorizationType;
using App.Areas.NhaMay.Models;
using App.Areas.NhaMay.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.NhaMay.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class NhaMayController : ControllerBase
{
    private readonly INhaMayService _nhaMayService;

    public NhaMayController(INhaMayService nhaMayService)
    {
        _nhaMayService = nhaMayService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieu(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _nhaMayService.LayNhieuAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                totalFactories = result.totalItems,
                factories = result.listItems
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
            var nhaMay = await _nhaMayService.LayMotBangIdAsync(id);

            return Ok(nhaMay);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("factory-code/{nm_MaNM}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayMotBangMaNhaMay(string nm_MaNM)
    {
        try
        {
            var nhaMay = await _nhaMayService.LayMotBangMaNhaMayAsync(nm_MaNM);

            return Ok(nhaMay);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> LayNhieuCuaToiAsync(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _nhaMayService.LayNhieuCuaToiAsync(User, pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                factories = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Them([FromBody] NhaMayModel nhaMay)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _nhaMayService.ThemAsync(nhaMay, User);

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
    public async Task<IActionResult> Update(Guid id, [FromBody] NhaMayModel nhaMay)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _nhaMayService.SuaAsync(id, nhaMay, User);

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
    public async Task<IActionResult> Xoa(Guid id)
    {
        try
        {
            await _nhaMayService.XoaAsync(id, User);

            return Ok("Xóa nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("enterprise/{id}")]
    public async Task<IActionResult> ThemDoanhNghiepVaoNhaMay(Guid id, [FromBody] Guid dn_id)
    {
        try
        {
            await _nhaMayService.ThemDoanhNghiepVaoNhaMayAsync(id, dn_id, User);

            return Ok("Thêm doanh nghiệp vào nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("enterprise/{id}")]
    public async Task<IActionResult> XoaDoanhNghiepKhoiNhaMay(Guid id)
    {
        try
        {
            await _nhaMayService.XoaDoanhNghiepKhoiNhaMayAsync(id, User);

            return Ok("Xóa doanh nghiệp sở hữu nhà máy thành công");
        }
        catch
        {
            throw;
        }
    }
}