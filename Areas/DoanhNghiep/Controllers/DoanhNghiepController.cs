using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Models;
using App.Areas.DoanhNghiep.Services;
using App.Areas.DTO;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DoanhNghiep.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
                tongSo = result.totalItems,
                listDoanhNghieps = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [AllowAnonymous]
    [HttpGet("co-ban")]
    public async Task<IActionResult> LayNhieuCoBan(int pageNumber, int limit, string? search, bool descending)
    {
        try
        {
            var result = await _doanhNghiepService.LayNhieuCoBanAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listDoanhNghieps = result.listItems
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
                tongSo = result.totalItems,
                listDoanhNghieps = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.DOANH_NGHIEP}")]
    public async Task<IActionResult> Them([FromBody] DoanhNghiepModel doanhNghiep)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var doanhNghiepNew = await _doanhNghiepService.ThemAsync(doanhNghiep, User);

                return Ok(doanhNghiepNew);
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

    [HttpPost("phan-quyen-san-pham/{id}")]
    public async Task<IActionResult> PhanQuyenSanPhamTheoDoanhNghiepAsync(Guid id, PhanQuyenDTO phanQuyenDTO)
    {
        try
        {
            await _doanhNghiepService.PhanQuyenSanPhamTheoDoanhNghiepAsync(id, phanQuyenDTO, User);

            return Ok("Phân quyền doanh nghiệp thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("avatar/{id}")]
    public async Task<IActionResult> TaiLenAvatarAsync(Guid id, IFormFile avatar)
    {
        try
        {
            await _doanhNghiepService.TaiLenAvatarDoanhNghiepAsync(id, avatar, User);

            return Ok("Tải lên ảnh đại diện thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("avatar/{id}")]
    public async Task<IActionResult> XoaAvatarAsync(Guid id)
    {
        try
        {
            await _doanhNghiepService.XoaAvatarDoanhNghiepAsync(id, User);

            return Ok("Xóa ảnh đại diện thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("cover-photo/{id}")]
    public async Task<IActionResult> TaiLenAnhBiaAsync(Guid id, IFormFile coverPhoto)
    {
        try
        {
            await _doanhNghiepService.TaiLenAnhBiaDoanhNghiepAsync(id, coverPhoto, User);

            return Ok("Tải lên ảnh bìa thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("cover-photo/{id}")]
    public async Task<IActionResult> XoaAnhBiaAsync(Guid id)
    {
        try
        {
            await _doanhNghiepService.XoaAnhBiaDoanhNghiepAsync(id, User);

            return Ok("Xóa ảnh bìa thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("kiem-tra-theo-doi/{id}")]
    public async Task<IActionResult> KiemTraDangTheoDoi(Guid id)
    {
        try
        {
            bool result = await _doanhNghiepService.KiemTraDangTheoDoiDoanhNghiepAsync(id, User);

            return Ok(result);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("theo-doi/{id}")]
    public async Task<IActionResult> TheoDoiHoacHuyTheoDoi(Guid id)
    {
        try
        {
            await _doanhNghiepService.TheoDoiHoacHuyTheoDoiDoanhNghiepAsync(id, User);

            return Ok();
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("so-luong-theo-doi/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> LaySoLuongTheoDoi(Guid id)
    {
        try
        {
            int soLuong = await _doanhNghiepService.LaySoTheoDoiAsync(id);

            return Ok(soLuong);
        }
        catch
        {
            throw;
        }
    }


}