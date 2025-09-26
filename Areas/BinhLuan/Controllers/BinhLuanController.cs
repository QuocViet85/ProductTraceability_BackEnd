using System.ComponentModel.DataAnnotations;
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

    [HttpGet("san-pham/{sp_id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangSanPham(Guid sp_id, [Range(0, 5)] int soSao, int pageNumber, int limit)
    {
        try
        {
            var result = await _binhLuanService.LayNhieuBangSanPhamAsync(sp_id, soSao, pageNumber, limit);

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

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangNguoiDung(Guid userId, int pageNumber, int limit)
    {
        try
        {
            var result = await _binhLuanService.LayNhieuBangNguoiDungAsync(userId, pageNumber, limit);

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

    [HttpPost("san-pham/{sp_id}")]
    [Authorize]
    public async Task<IActionResult> Them(Guid sp_id, [FromForm] string noiDung, List<IFormFile>? listImages)
    {
        try
        {
            var binhLuanNew = await _binhLuanService.ThemAsync(sp_id, noiDung, listImages, User);

            return Ok(binhLuanNew);
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