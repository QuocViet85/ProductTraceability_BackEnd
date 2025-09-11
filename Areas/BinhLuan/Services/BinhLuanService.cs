using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.Auth.Mapper;
using App.Areas.BinhLuan.Models;
using App.Areas.BinhLuan.Repositories;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;
using App.Areas.SanPham.Repositories;

namespace App.Areas.BinhLuan.Services;

public class BinhLuanService : IBinhLuanService
{
    private readonly IBinhLuanRepository _binhLuanRepo;

    private readonly IFileService _fileService;

    private readonly ISanPhamRepository _sanPhamRepo;

    public BinhLuanService(IBinhLuanRepository binhLuanRepo, IFileService fileService, ISanPhamRepository sanPhamRepo)
    {
        _binhLuanRepo = binhLuanRepo;
        _fileService = fileService;
        _sanPhamRepo = sanPhamRepo;
    }

    public async Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit)
    {
        int tongSo = await _binhLuanRepo.LayTongSoBangSanPhamAsync(sp_id);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BinhLuanModel> listBinhLuans = await _binhLuanRepo.LayNhieuBangSanPhamAsync(sp_id, pageNumber, limit);

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

    public async Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangNguoiDungAsync(Guid userId, int pageNumber, int limit)
    {
        int tongSo = await _binhLuanRepo.LayTongSoBangNguoiDungAsync(userId);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BinhLuanModel> listBinhLuans = await _binhLuanRepo.LayNhieuBangNguoiDungAsync(userId, pageNumber, limit);

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

    public async Task ThemAsync(Guid sp_id, string noiDung, List<IFormFile>? listImages, ClaimsPrincipal userNowFromJwt)
    {
        var existSanPham = await _sanPhamRepo.KiemTraTonTaiBangIdAsync(sp_id);

        if (!existSanPham)
        {
            throw new Exception("Sản phẩm không tồn tại");
        }

        if (string.IsNullOrEmpty(noiDung))
        {
            throw new Exception("Bình luận không có nội dung");
        }

        var binhLuanModel = new BinhLuanModel()
        {
            BL_NoiDung = noiDung,
            BL_SP_Id = sp_id,
            BL_NguoiTao_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        };

        int result = await _binhLuanRepo.ThemAsync(binhLuanModel);

        if (result == 0)
        {
            throw new Exception("Thêm bình luận thất bại");
        }

        if (listImages != null) {
            if (listImages.Count <= 5)
            {
                await _fileService.TaiLenAsync(listImages, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.BINH_LUAN, binhLuanModel.BL_Id, userNowFromJwt);
            }
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