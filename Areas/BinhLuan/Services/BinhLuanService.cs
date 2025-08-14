using System.Diagnostics;
using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.Auth.Mapper;
using App.Areas.BinhLuan.Models;
using App.Areas.BinhLuan.Repositories;
using App.Areas.SanPham.Repositories;
using Areas.Auth.DTO;

namespace App.Areas.BinhLuan.Services;

public class BinhLuanService : IBinhLuanService
{
    private readonly IBinhLuanRepository _binhLuanRepo;
    private readonly ISanPhamRepository _sanPhamRepo;

    public BinhLuanService(IBinhLuanRepository binhLuanRepo, ISanPhamRepository sanPhamRepo)
    {
        _binhLuanRepo = binhLuanRepo;
        _sanPhamRepo = sanPhamRepo;
    }

    public async Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit)
    {
        int tongSo = await _binhLuanRepo.LayTongSoBangSanPhamAsync(sp_Id);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BinhLuanModel> listBinhLuans = await _binhLuanRepo.LayNhieuBangSanPhamAsync(sp_Id, pageNumber, limit);

        return (tongSo, listBinhLuans);
    }
    public async Task ThemAsync(BinhLuanModel binhLuan, ClaimsPrincipal userNowFromJwt)
    {
        bool existProduct = await _sanPhamRepo.KiemTraTonTaiBangIdAsync(binhLuan.BL_SP_Id);
        if (!existProduct)
        {
            throw new Exception("Sản phẩm không tồn tại");
        }

        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        binhLuan.BL_NguoiTao_Id = Guid.Parse(userIdNow);
        binhLuan.BL_NgayTao = DateTime.Now;

        int result = await _binhLuanRepo.ThemAsync(binhLuan);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo bình luận thất bại");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var binhLuan = await _binhLuanRepo.LayMotBangIdAsync(id);

        if (binhLuan == null)
        {
            throw new Exception("Bình luận không tồn tại");
        }

        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userNowFromJwt.IsInRole(Roles.ADMIN) || binhLuan.BL_NguoiTao_Id.ToString() == userIdNow)
        {
            int result = await _binhLuanRepo.XoaAsync(binhLuan);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo bình luận thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa bình luận");
        }
    }

    //Not Implement
    public Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<BinhLuanModel> LayMotBangIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task SuaAsync(Guid id, BinhLuanModel TModel, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }
}