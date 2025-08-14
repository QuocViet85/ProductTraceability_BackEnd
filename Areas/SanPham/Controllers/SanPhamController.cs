using App.Areas.Auth.AuthorizationData;
using App.Areas.SanPham.Models;
using App.Areas.SanPham.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.SanPham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class SanPhamController : ControllerBase
{
    private readonly ISanPhamService _sanPhamService;

    public SanPhamController(ISanPhamService sanPhamService)
    {
        _sanPhamService = sanPhamService;
    }

    [HttpGet]
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpGet("me")]
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
    public async Task<IActionResult> Them([FromBody] SanPhamModel sanPham)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _sanPhamService.ThemAsync(sanPham, User);

                return Ok("Tạo sản phẩm thành công");
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpGet("doanh-nghiep-van-tai/{dn_id}")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpGet("nguoi-phu-trach/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> LayNhieuBangNguoiPhuTrach(Guid userId, int pageNumber, int limit, string? search, bool descending = true)
    {
        try
        {
            var result = await _sanPhamService.LayNhieuBangNguoiPhuTrachAsync(userId, pageNumber, limit, search, descending);

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
    [AllowAnonymous]
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


    [HttpPost("doanh-nghiep-so-huu/{id}")]
    public async Task<IActionResult> ThemDoanhNghiepSoHuuSanPham(Guid id, [FromBody] Guid dn_id)
    {
        try
        {
            await _sanPhamService.ThemDoanhNghiepSoHuuSanPhamAsync(id, dn_id, User);

            return Ok("Thêm doanh nghiệp sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("doanh-nghiep-so-huu/{id}")]
    public async Task<IActionResult> XoaDoanhNghiepSoHuuSanPham(Guid id)
    {
        try
        {
            await _sanPhamService.XoaDoanhNghiepSoHuuSanPhamAsync(id, User);

            return Ok("Xóa doanh nghiệp sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("doanh-nghiep-san-xuat/{id}")]
    public async Task<IActionResult> ThemDoanhNghiepSanXuatSanPham(Guid id, [FromBody] Guid dn_id)
    {
        try
        {
            await _sanPhamService.ThemDoanhNghiepSanXuatSanPhamAsync(id, dn_id, User);

            return Ok("Thêm doanh nghiệp sản xuất sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("doanh-nghiep-san-xuat/{id}")]
    public async Task<IActionResult> XoaDoanhNghiepSanXuatSanPham(Guid id)
    {
        try
        {
            await _sanPhamService.XoaDoanhNghiepSanXuatSanPhamAsync(id, User);

            return Ok("Xóa doanh nghiệp sản xuất sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("doanh-nghiep-van-tai/{id}")]
    public async Task<IActionResult> ThemDoanhNghiepVanTaiSanPham(Guid id, [FromBody] Guid dn_id)
    {
        try
        {
            await _sanPhamService.ThemDoanhNghiepVanTaiSanPhamAsync(id, dn_id, User);

            return Ok("Thêm doanh nghiệp vận tải sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("doanh-nghiep-van-tai/{id}")]
    public async Task<IActionResult> XoaDoanhNghiepVanTaiSanPham(Guid id)
    {
        try
        {
            await _sanPhamService.XoaDoanhNghiepVanTaiSanPhamAsync(id, User);

            return Ok("Xóa doanh nghiệp vận tải sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("nguoi-phu-trach/{id}")]
    public async Task<IActionResult> ThemNguoiPhuTrachSanPha(Guid id, [FromBody] Guid userId)
    {
        try
        {
            await _sanPhamService.ThemNguoiPhuTrachSanPhamAsync(id, userId, User);

            return Ok("Thêm người phụ trách sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("nguoi-phu-trach/{id}")]
    public async Task<IActionResult> XoaNguoiPhuTrachSanPham(Guid id)
    {
        try
        {
            await _sanPhamService.XoaNguoiPhuTrachSanPhamAsync(id, User);

            return Ok("Xóa người phụ trách sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("nha-may/{id}")]
    public async Task<IActionResult> ThemNhaMayCuaSanPham(Guid id, [FromBody] Guid dn_id)
    {
        try
        {
            await _sanPhamService.ThemNhaMayCuaSanPhamAsync(id, dn_id, User);

            return Ok("Thêm nhà máy của sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("nha-may/{id}")]
    public async Task<IActionResult> XoaNhaMayCuaSanPham(Guid id)
    {
        try
        {
            await _sanPhamService.XoaNhaMayCuaSanPhamAsync(id, User);

            return Ok("Xóa nhà máy của sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("photos/{id}")]
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
}