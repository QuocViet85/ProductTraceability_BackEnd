using App.Areas.BinhLuan.Models;
using App.Areas.BinhLuan.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.BinhLuan.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BinhLuanController : ControllerBase
{
    private readonly IBinhLuanService _binhLuanService;

    public BinhLuanController(IBinhLuanService binhLuanService)
    {
        _binhLuanService = binhLuanService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangTaiNguyen(string kieuTaiNguyen, Guid taiNguyenId, int pageNumber, int limit)
    {
        try
        {
            var result = await _binhLuanService.LayNhieuBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId, pageNumber, limit);

            return Ok(new
            {
                tongSo = result.totalItems,
                listBinhLuans = result.listItems
            });
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
            await _binhLuanService.XoaAsync(id, User);

            return Ok("Xóa bình luận thành công");
        }
        catch
        {
            throw;
        }
    }

}