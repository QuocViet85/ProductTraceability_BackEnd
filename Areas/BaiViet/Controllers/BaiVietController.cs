using App.Areas.BaiViet.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.BaiViet.Services;

[ApiController]
[Route("api/[controller]")]
public class BaiVietController : ControllerBase
{
    private IBaiVietService _baiVietService;

    public BaiVietController(IBaiVietService baiVietService)
    {
        _baiVietService = baiVietService;
    }

    [HttpGet]
    public async Task<IActionResult> LayNhieu(int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _baiVietService.LayNhieuAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listBaiViets = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("san-pham/{sp_id}")]
    public async Task<IActionResult> LayNhieuBangSanPham(Guid sp_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _baiVietService.LayNhieuBangSanPhamAsync(sp_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listBaiViets = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> LayNhieuCuaNguoiDung(Guid userId, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _baiVietService.LayNhieuCuaNguoiDungAsync(userId, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listBaiViets = result.listItems
            });
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
            var baiViet = await _baiVietService.LayMotBangIdAsync(id);

            return Ok(baiViet);
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Them([FromBody] BaiVietModel baiViet)
    {
        try
        {
            var baiVietNew = await _baiVietService.ThemAsync(baiViet, User);

            return Ok(baiVietNew);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Sua(Guid id, [FromBody] BaiVietDTO baiVietDTO)
    {
        try
        {
            await _baiVietService.SuaAsync(id, baiVietDTO, User);

            return Ok("Sửa bài viết thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Xoa(Guid id)
    {
        try
        {
            await _baiVietService.XoaAsync(id, User);

            return Ok("Xóa bài viết thành công");
        }
        catch
        {
            throw;
        }
    }
}

public class BaiVietDTO
{
    public string TieuDe { set; get; }
    public string NoiDung { set; get; }
}