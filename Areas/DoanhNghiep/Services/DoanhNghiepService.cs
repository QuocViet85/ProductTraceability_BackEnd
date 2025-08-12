using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.DoanhNghiep.Auth;
using App.Messages;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.DoanhNghiep.Models;

namespace App.Areas.DoanhNghiep.Services;

public class DoanhNghiepService : IDoanhNghiepService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;

    public DoanhNghiepService(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IDoanhNghiepRepository doanhNghiepRepo)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _doanhNghiepRepo = doanhNghiepRepo;
    }

    public async Task<(int totalItems, List<DoanhNghiepModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _doanhNghiepRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<DoanhNghiepModel> listDoanhNghieps = await _doanhNghiepRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listDoanhNghieps);
    }
    public async Task<(int totalItems, List<DoanhNghiepModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int tongSo = await _doanhNghiepRepo.LayTongSoCuaNguoiDungAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<DoanhNghiepModel> listDoanhNghieps = await _doanhNghiepRepo.LayNhieuCuaNguoiDungAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        return (tongSo, listDoanhNghieps);
    }

    public async Task<DoanhNghiepModel> LayMotBangIdAsync(Guid id)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);
        if (doanhNghiep == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return doanhNghiep;
    }

    public async Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string maSoThue)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangMaSoThueAsync(maSoThue);
        if (doanhNghiep == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return doanhNghiep;
    }

    public async Task ThemAsync(DoanhNghiepModel doanhNghiep, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaSoThueAsync(doanhNghiep.DN_MaSoThue) || await _doanhNghiepRepo.KiemTraTonTaiBangMaGLNAsync(doanhNghiep.DN_MaGLN))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        doanhNghiep.DN_NguoiTaoId = Guid.Parse(userIdNow);

        if (doanhNghiep.DN_KieuDN == null)
        {
            throw new Exception("Phải chọn kiểu doanh nghiệp");
        }

        int result = await _doanhNghiepRepo.ThemAsync(doanhNghiep);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo doanh nghiệp thất bại");
        }

        var chuDoanhNghiep = new ChuDoanhNghiepModel()
        {
            CDN_DN_Id = doanhNghiep.DN_Id,
            CDN_ChuDN_Id = Guid.Parse(userIdNow),
        };

        await _doanhNghiepRepo.ThemSoHuuDoanhNghiepAsync(chuDoanhNghiep);
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var quyenXoa = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaXoaDoanhNghiepRequirement(xoa: true));

        if (quyenXoa.Succeeded)
        {
            int result = await _doanhNghiepRepo.XoaAsync(doanhNghiep);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(quyenXoa.Failure.FailureReasons));
        }
    }

    public async Task SuaAsync(Guid id, DoanhNghiepModel doanhNghiepUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);

        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (await _doanhNghiepRepo.KiemTraTonTaiBangMaSoThueAsync(doanhNghiepUpdate.DN_MaSoThue, id) || await _doanhNghiepRepo.KiemTraTonTaiBangMaGLNAsync(doanhNghiepUpdate.DN_MaGLN))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaXoaDoanhNghiepRequirement());

        if (quyenSua.Succeeded)
        {
            doanhNghiep.DN_Ten = doanhNghiepUpdate.DN_Ten;
            doanhNghiep.DN_MaSoThue = doanhNghiepUpdate.DN_MaSoThue;
            doanhNghiep.DN_MaGLN = doanhNghiepUpdate.DN_MaGLN;
            doanhNghiep.DN_DiaChi = doanhNghiepUpdate.DN_DiaChi;
            doanhNghiep.DN_SoDienThoai = doanhNghiepUpdate.DN_SoDienThoai;
            doanhNghiep.DN_Email = doanhNghiepUpdate.DN_Email;
            doanhNghiep.DN_JsonData = doanhNghiepUpdate.DN_JsonData;

            doanhNghiep.DN_NgaySua = DateTime.Now;
            doanhNghiep.DN_NguoiSuaId = Guid.Parse(userIdNow);

            int result = await _doanhNghiepRepo.SuaAsync(doanhNghiep);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(quyenSua.Failure.FailureReasons));
        }
    }

    public async Task ThemSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        bool daLaCDN = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync(id, userId);
        if (daLaCDN)
        {
            throw new Exception("User này đang sở hữu doanh nghiệp này");
        }

        var userAdd = await _userManager.FindByIdAsync(userId.ToString());
        if (userAdd == null)
        {
            throw new Exception("Không tìm thấy User để thêm sở hữu");
        }

        var doanhNghiep = await _doanhNghiepRepo.LayMotBangIdAsync(id);
        if (doanhNghiep == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (doanhNghiep.DN_KieuDN == 1 && doanhNghiep.DN_List_CDN.Count == 1)
        {
            throw new Exception("Không thể thêm chủ doanh nghiệp cho doanh nghiệp cá nhân đã có chủ");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, doanhNghiep, new SuaXoaDoanhNghiepRequirement());

        if (quyenSua.Succeeded)
        {
            string roleUserAdd = (await _userManager.GetRolesAsync(userAdd))[0];

            if (roleUserAdd == Roles.ADMIN || roleUserAdd == Roles.ENTERPRISE)
            {
                var chuDoanhNghiep = new ChuDoanhNghiepModel()
                {
                    CDN_DN_Id = doanhNghiep.DN_Id,
                    CDN_ChuDN_Id = userAdd.Id
                };

                int result = await _doanhNghiepRepo.ThemSoHuuDoanhNghiepAsync(chuDoanhNghiep);

                if (result == 0)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu. Không thể cập nhật sở hữu doanh nghiệp");
                }
            }
            else
            {
                throw new Exception($"User với vai trò {roleUserAdd} không được sở hữu doanh nghiệp");
            }
        }
        else
        {
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(quyenSua.Failure.FailureReasons));
        }
    }

    public async Task TuBoSoHuuDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int result = await _doanhNghiepRepo.TuBoSoHuuDoanhNghiepAsync(id, Guid.Parse(userIdNow));

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể từ bỏ sở hữu doanh nghiệp");
        }
    }

    public async Task XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId)
    {
        int result = await _doanhNghiepRepo.XoaSoHuuDoanhNghiepAsync(id, userId);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể xóa sở hữu doanh nghiệp");
        }
    }
}