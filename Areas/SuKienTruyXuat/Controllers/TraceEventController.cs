using App.Areas.Auth.AuthorizationType;
using App.Areas.SuKienTruyXuat.Models;
using App.Areas.SuKienTruyXuat.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.SuKienTruyXuat.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class SuKienTruyXuatController : ControllerBase
{
    private readonly ISuKienTruyXuatService _suKienTruyXuatService;

    public SuKienTruyXuatController(ISuKienTruyXuatService suKienTruyXuatService)
    {
        _suKienTruyXuatService = suKienTruyXuatService;
    }

    [HttpGet("batch/{lsp_Id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangLoSanPham(Guid lsp_Id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _suKienTruyXuatService.LayNhieuBangLoSanPhamAsync(lsp_Id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSuKienTruyXuat = result.listItems
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
            var suKienTruyXuat = await _suKienTruyXuatService.LayMotBangIdAsync(id);

            return Ok(suKienTruyXuat);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-su-kien/{sk_maSK}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByTraceEventCode(string sk_maSK)
    {
        try
        {
            var suKienTruyXuat = await _suKienTruyXuatService.LayMotBangMaSuKienAsync(sk_maSK);

            return Ok(suKienTruyXuat);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Them([FromBody] SuKienTruyXuatModel suKienTruyXuat)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _suKienTruyXuatService.ThemAsync(suKienTruyXuat, User);

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

    [HttpPut("{id}")]
    public async Task<IActionResult> Sua(Guid id, [FromBody] SuKienTruyXuatModel suKienTruyXuat)
    {
        try
        {
            await _suKienTruyXuatService.SuaAsync(id, suKienTruyXuat, User);

            return Ok("Cập nhật sự kiện truy xuất thành công");
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
            await _suKienTruyXuatService.XoaAsync(id, User);

            return Ok("Xóa sự kiện truy xuất thành công");
        }
        catch
        {
            throw;
        }
    }
}