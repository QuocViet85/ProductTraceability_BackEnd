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

    [HttpGet("san-pham/{sp_Id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieu(Guid sp_Id, int pageNumber, int limit)
    {
        try
        {
            var result = await _binhLuanService.LayNhieuBangSanPhamAsync(sp_Id, pageNumber, limit);

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

    [HttpPost()]
    public async Task<IActionResult> Them([FromBody] BinhLuanModel binhLuan)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _binhLuanService.ThemAsync(binhLuan, User);

                return Ok("Tạo bình luận thành công");
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
            await _binhLuanService.XoaAsync(id, User);

            return Ok("Xóa bình luận thành công");
        }
        catch
        {
            throw;
        }
    }

}