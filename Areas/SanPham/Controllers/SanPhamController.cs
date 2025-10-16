using System.ComponentModel.DataAnnotations;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DTO;
using App.Areas.SanPham.Models;
using App.Areas.SanPham.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.SanPham.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SanPhamController : ControllerBase
{
    private readonly ISanPhamService _sanPhamService;

    public SanPhamController(ISanPhamService sanPhamService)
    {
        _sanPhamService = sanPhamService;
    }

    [HttpGet]
    public async Task<IActionResult> LayNhieu(int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuAsync(pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
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
            var sanPham = await _sanPhamService.LayMotBangIdAsync(id);

            return Ok(sanPham);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-truy-xuat/{maTruyXuat}")]
    public async Task<IActionResult> LayMotBangMaTruyXuat(string maTruyXuat)
    {
        try
        {
            var sanPham = await _sanPhamService.LayMotBangMaTruyXuatAsync(maTruyXuat);

            return Ok(sanPham);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-truy-xuat/ton-tai/{maTruyXuat}")]
    public async Task<IActionResult> KiemTraTonTaiBangMaTruyXuat(string maTruyXuat)
    {
        try
        {
            var tonTai = await _sanPhamService.KiemTraTonTaiBangMaTruyXuatAsync(maTruyXuat);

            return Ok(tonTai);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-vach/{maVach}")]
    public async Task<IActionResult> LayMotBangMaVach(string maVach)
    {
        try
        {
            var sanPham = await _sanPhamService.LayMotBangMaVachAsync(maVach);

            return Ok(sanPham);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("ma-vach/ton-tai/{maVach}")]
    public async Task<IActionResult> KiemTraTonTaiBangMaVach(string maVach)
    {
        try
        {
            var tonTai = await _sanPhamService.KiemTraTonTaiBangMaVachAsync(maVach);

            return Ok(tonTai);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("me")]
    [Authorize(Roles = $"{Roles.ADMIN}, {Roles.DOANH_NGHIEP}")]
    public async Task<IActionResult> LayNhieuCuaToi(int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuCuaToiAsync(User, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Them([FromBody] SanPhamModel sanPham)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanPhamNew = await _sanPhamService.ThemAsync(sanPham, User);

                return Ok(sanPhamNew);
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
    [Authorize]
    public async Task<IActionResult> Sua(Guid id, [FromBody] SanPhamModel sanPham)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _sanPhamService.SuaAsync(id, sanPham, User);

                return Ok("Cập nhật sản phẩm thành công");
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
    [Authorize]
    public async Task<IActionResult> Xoa(Guid id)
    {
        try
        {
            await _sanPhamService.XoaAsync(id, User);

            return Ok("Xóa sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("danh-muc/{dm_id}")]
    public async Task<IActionResult> LayNhieuBangDanhMuc(Guid dm_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangDanhMucAsync(dm_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("doanh-nghiep-so-huu/{dn_id}")]
    public async Task<IActionResult> LayNhieuBangDoanhNghiepSoHuu(Guid dn_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangDoanhNghiepSoHuuAsync(dn_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("doanh-nghiep-so-huu/tong-so/{dn_id}")]
    public async Task<IActionResult> LayTongSoBangDoanhNghiepSoHuu(Guid dn_id)
    {
        try
        {
            var tongSo = await _sanPhamService.LayTongSoBangDoanhNghiepSoHuuAsync(dn_id);

            return Ok(tongSo);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("doanh-nghiep-van-tai/{dn_id}")]
    public async Task<IActionResult> LayNhieuBangDoanhNghiepVanTai(Guid dn_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangDoanhNghiepVanTaiAsync(dn_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("doanh-nghiep-san-xuat/{dn_id}")]
    public async Task<IActionResult> LayNhieuBangDoanhNghiepSanXuat(Guid dn_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangDoanhNghiepSanXuatAsync(dn_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("nha-may/{nm_id}")]
    public async Task<IActionResult> LayNhieuBangNhaMay(Guid nm_id, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangNhaMayAsync(nm_id, pageNumber, limit, search, descending);

            return Ok(new
            {
                tongSo = result.totalItems,
                listSanPhams = result.listItems
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("photos/{id}")]
    [Authorize]
    public async Task<IActionResult> TaiLenAnhSanPham(Guid id, List<IFormFile> listFiles)
    {
        try
        {
            await _sanPhamService.TaiLenAnhSanPhamAsync(id, listFiles, User);

            return Ok("Upload ảnh thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("photos/{id}")]
    [Authorize]
    public async Task<IActionResult> XoaAnhSanPham(Guid id, Guid f_id)
    {
        try
        {
            await _sanPhamService.XoaAnhSanPhamAsync(id, f_id, User);

            return Ok("Xóa ảnh thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("sao-san-pham/{id}")]
    [Authorize]
    public async Task<IActionResult> ThemSaoSanPham(Guid id, [Range(0, 5)] int soSao)
    {
        try
        {
            await _sanPhamService.ThemSaoAsync(id, soSao, User);

            return Ok();
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("sao-san-pham/{id}")]
    public async Task<IActionResult> LaySoSaoSanPham(Guid id)
    {
        try
        {
            double soSao = await _sanPhamService.LaySoSaoAsync(id);

            return Ok(soSao);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("sao-san-pham-user/{id}")]
    public async Task<IActionResult> LaySoSaoSanPham(Guid id, Guid userId)
    {
        try
        {
            int soSao = await _sanPhamService.LaySoSaoCuaMotUserAsync(id, userId);

            return Ok(soSao);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("doanh-nghiep-so-huu-id/{id}")]
    public async Task<IActionResult> LayDoanhNghiepSoHuuId(Guid id)
    {
        try
        {
            Guid? doanhNghiepId = await _sanPhamService.LayDoanhNghiepIdSoHuuSanPhamAsync(id);

            return Ok(doanhNghiepId);
        }
        catch
        {
            throw;
        }
    }

}