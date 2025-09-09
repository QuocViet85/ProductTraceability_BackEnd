using System.Diagnostics;
using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.Auth.Mapper;
using App.Areas.BinhLuan.Models;
using App.Areas.BinhLuan.Repositories;
using App.Areas.Files.Repositories;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;
using App.Areas.SanPham.Repositories;
using Areas.Auth.DTO;

namespace App.Areas.BinhLuan.Services;

public class BinhLuanService : IBinhLuanService
{
    private readonly IBinhLuanRepository _binhLuanRepo;

    private readonly IFileService _fileService;

    public BinhLuanService(IBinhLuanRepository binhLuanRepo, IFileService fileService)
    {
        _binhLuanRepo = binhLuanRepo;
        _fileService = fileService;
    }

    public async Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, int pageNumber, int limit)
    {
        int tongSo = await _binhLuanRepo.LayTongSoBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BinhLuanModel> listBinhLuans = await _binhLuanRepo.LayNhieuBangTaiNguyenAsync(kieuTaiNguyen, taiNguyenId, pageNumber, limit);

        foreach (var binhLuan in listBinhLuans)
        {
            if (binhLuan.BL_NguoiTao != null)
            {
                binhLuan.BL_NguoiTao_Client = UserMapper.ModelToDto(binhLuan.BL_NguoiTao);
                binhLuan.BL_NguoiTao = null;
            }
        }

        return (tongSo, listBinhLuans);
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
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

            await _fileService.XoaNhieuBangTaiNguyenAsync(KieuTaiNguyen.BINH_LUAN, id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa bình luận");
        }
    }

    //Not Implement
    public async Task ThemAsync(BinhLuanModel binhLuan, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

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
}