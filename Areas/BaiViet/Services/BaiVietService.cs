using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.BaiViet.Model;
using App.Areas.BaiViet.Repositories;
using App.Areas.SanPham.Repositories;

namespace App.Areas.BaiViet.Services;

public class BaiVietService : IBaiVietService
{
    private readonly IBaiVietRepository _baiVietRepo;
    private readonly ISanPhamRepository _sanPhamRepo;

    public BaiVietService(IBaiVietRepository baiVietRepo, ISanPhamRepository sanPhamRepo)
    {
        _baiVietRepo = baiVietRepo;
        _sanPhamRepo = sanPhamRepo;
    }

    public async Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _baiVietRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BaiVietModel> listBaiViets = await _baiVietRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listBaiViets);
    }

    public async Task<BaiVietModel> LayMotBangIdAsync(Guid id)
    {
        var baiViet = await _baiVietRepo.LayMotBangIdAsync(id);

        if (baiViet == null)
        {
            throw new Exception("Không tìm thấy bài viết");
        }

        return baiViet;
    }

    public async Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _baiVietRepo.LayTongSoBangSanPhamAsync(sp_id);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BaiVietModel> listBaiViets = await _baiVietRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listBaiViets);
    }

    public async Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _baiVietRepo.LayTongSoCuaNguoiDungAsync(userId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BaiVietModel> listBaiViets = await _baiVietRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listBaiViets);
    }

    public async Task ThemAsync(BaiVietModel baiViet, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (await _sanPhamRepo.KiemTraTonTaiBangIdAsync(baiViet.BV_SP_Id))
        {
            baiViet.BV_NguoiTao_Id = userIdNow;

            int result = await _baiVietRepo.ThemAsync(baiViet);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm bài viết thất bại");
            }
        }
        else
        {
            throw new Exception("Không tồn tại sản phẩm");
        }
    }

    public async Task SuaAsync(Guid id, string noiDung, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var baiVietModel = await _baiVietRepo.LayMotBangIdAsync(id);

        if (baiVietModel == null)
        {
            throw new Exception("Không tồn tại bài viết");
        }

        if (baiVietModel.BV_NguoiTao_Id == userIdNow)
        {
            if (baiVietModel.BV_NoiDung != noiDung)
            {
                baiVietModel.BV_NoiDung = noiDung;
                baiVietModel.BV_NgaySua = DateTime.Now;
                int result = await _baiVietRepo.SuaAsync(baiVietModel);

                if (result == 0)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu. Sửa bài viết thất bại");
                }
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa bài viết không phải của mình");
        }
    }
    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var baiVietModel = await _baiVietRepo.LayMotBangIdAsync(id);

        if (baiVietModel == null)
        {
            throw new Exception("Không tồn tại bài viết");
        }

        if (userNowFromJwt.IsInRole(Roles.ADMIN) || baiVietModel.BV_NguoiTao_Id == userIdNow)
        {
            int result = await _baiVietRepo.XoaAsync(baiVietModel);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa bài viết thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa bài viết");
        }
    }
    
    //Not Implements
    public Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task SuaAsync(Guid id, BaiVietModel TModel, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }
}
