using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.DTO;
using App.Areas.DoanhNghiep.Models;
using App.Areas.DoanhNghiep.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DoanhNghiep.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoanhNghiepController : ControllerBase
{
    private readonly IDoanhNghiepService _doanhNghiepService;

    public DoanhNghiepController(IDoanhNghiepService enterpriseService)
    {
        _doanhNghiepService = enterpriseService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> LayNhieu(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _doanhNghiepService.LayNhieuAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                enterprises = result.listItems
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
            var doanhNghiep = await _doanhNghiepService.LayMotBangIdAsync(id);

            return Ok(doanhNghiep);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-so-thue/{dn_MaSoThue}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayMotBangMaSoThue(string dn_MaSoThue)
    {
        try
        {
            var doanhNghiep = await _doanhNghiepService.LayMotBangMaSoThueAsync(dn_MaSoThue);

            return Ok(doanhNghiep);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> LayNhieuCuaToi(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _doanhNghiepService.LayNhieuCuaToiAsync(User, pageNumber, limit, search, descending);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                enterprises = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.ENTERPRISE}")]
    public async Task<IActionResult> Them([FromBody] DoanhNghiepModel doanhNghiep)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _doanhNghiepService.ThemAsync(doanhNghiep, User);

                return Ok("Tạo doanh nghiệp thành công");
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
    public async Task<IActionResult> Sua(Guid id, [FromBody] DoanhNghiepModel doanhNghiep)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _doanhNghiepService.SuaAsync(id, doanhNghiep, User);

                return Ok("Cập nhật doanh nghiệp thành công");
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
            await _doanhNghiepService.XoaAsync(id, User);

            return Ok("Xóa doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("so-huu/{id}")]
    public async Task<IActionResult> ThemSoHuuDoanhNghiep(Guid id, [FromBody] Guid userId)
    {
        try
        {
            await _doanhNghiepService.ThemSoHuuDoanhNghiepAsync(id, userId, User);

            return Ok("Thêm sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("so-huu/me/{id}")]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.ENTERPRISE}")]
    public async Task<IActionResult> TuBoSoHuuDoanhNghiep(Guid id)
    {
        try
        {
            await _doanhNghiepService.TuBoSoHuuDoanhNghiepAsync(id, User);

            return Ok("Từ bỏ sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("so-huu/{id}")]
    public async Task<IActionResult> XoaSoHuuDoanhNghiep(Guid id, [FromBody] Guid userId)
    {
        try
        {
            await _doanhNghiepService.XoaSoHuuDoanhNghiepAsync(id, userId, User);

            return Ok("Xóa sở hữu doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("phan-quyen/{id}")]
    public async Task<IActionResult> PhanQuyenDoanhNghiep(Guid id, PhanQuyenDTO phanQuyenDTO)
    {
        try
        {
            await _doanhNghiepService.PhanQuyenDoanhNghiepAsync(id, phanQuyenDTO, User);

            return Ok("Phân quyền doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }
}