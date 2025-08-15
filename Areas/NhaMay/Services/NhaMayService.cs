using System.Security.Claims;
using App.Areas.NhaMay.Models;
using App.Areas.NhaMay.Repositories;
using App.Areas.DoanhNghiep.Repositories;
using Microsoft.AspNetCore.Authorization;
using App.Areas.NhaMay.Authorization;
using App.Helper;
using App.Areas.DTO;
using App.Areas.Auth.AuthorizationData;
using Microsoft.AspNetCore.Identity;
using App.Database;

namespace App.Areas.NhaMay.Services;

public class NhaMayService : INhaMayService
{
    private readonly INhaMayRepository _nhaMayRepo;
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<AppUser> _userManager;

    public NhaMayService(INhaMayRepository nhaMayRepo, IDoanhNghiepRepository doanhNghiepRepo, IAuthorizationService authorizationService, UserManager<AppUser> userManager)
    {
        _nhaMayRepo = nhaMayRepo;
        _doanhNghiepRepo = doanhNghiepRepo;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }

    public async Task<(int totalItems, List<NhaMayModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        int tongSo = await _nhaMayRepo.LayTongSoAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<NhaMayModel> listNhaMays = await _nhaMayRepo.LayNhieuAsync(pageNumber, limit, search, descending);

        return (tongSo, listNhaMays);
    }

    public async Task<(int totalItems, List<NhaMayModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int tongSo = await _nhaMayRepo.LayTongSoCuaNguoiDungAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<NhaMayModel> listNhaMays = await _nhaMayRepo.LayNhieuCuaNguoiDungAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        return (tongSo, listNhaMays);
    }

    public async Task<NhaMayModel> LayMotBangIdAsync(Guid id)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);
        if (nhaMay == null)
        {
            throw new Exception("Không tìm thấy nhà máy");
        }

        return nhaMay;
    }

    public async Task ThemAsync(NhaMayModel nhaMayModel, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null)
        {
            throw new Exception("Tài khoản không hợp lệ");
        }

        if (nhaMayModel.NM_DN_Id != null)
        {
            var tonTaiDoanhNghiepSoHuuNhaMay = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync((Guid)nhaMayModel.NM_DN_Id);

            if (!tonTaiDoanhNghiepSoHuuNhaMay)
            {
                nhaMayModel.NM_DN_Id = null;
            }
        }

        if (nhaMayModel.NM_MaNM != null)
        {
            bool daCoMaNhaMay = await _nhaMayRepo.KiemTraTonTaiBangMaNhaMayAsync(nhaMayModel.NM_MaNM);

            if (daCoMaNhaMay)
            {
                throw new Exception("Mã nhà máy đã tồn tại nên không tạo nhà máy");
            }
        }
        else
        {
            nhaMayModel.NM_MaNM = CreateCode.GenerateCodeFromTicks();
        }

        nhaMayModel.NM_NguoiTao_Id = Guid.Parse(userIdNow);

        int result = await _nhaMayRepo.ThemAsync(nhaMayModel);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo nhà máy thất bại");
        }

        if (userNowFromJwt.IsInRole(Roles.DOANH_NGHIEP))
        {
            var adminNhaMayClaim = new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Admin, nhaMayModel.NM_Id));

            await _userManager.AddClaimAsync(user, adminNhaMayClaim);
        }
    }

    public async Task SuaAsync(Guid id, NhaMayModel nhaMayUpdate, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Nhà máy không tồn tại");
        }

        var quyenSua = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new SuaNhaMayRequirement());

        if (quyenSua.Succeeded)
        {
            if (nhaMayUpdate.NM_MaNM != null)
            {
                bool daCoMaNhaMay = await _nhaMayRepo.KiemTraTonTaiBangMaNhaMayAsync(nhaMayUpdate.NM_MaNM);

                if (daCoMaNhaMay)
                {
                    throw new Exception("Mã nhà máy đã tồn tại nên không cập nhật nhà máy");
                }

                nhaMay.NM_MaNM = nhaMayUpdate.NM_MaNM;
            }
            nhaMay.NM_Ten = nhaMayUpdate.NM_Ten;
            nhaMay.NM_LienHe = nhaMayUpdate.NM_LienHe;
            nhaMay.NM_DiaChi = nhaMayUpdate.NM_DiaChi;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật nhà máy này");
        }
    }

    public async Task ThemDoanhNghiepVaoNhaMayAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt)
    {
        var tonTaiDoanhNghiep = await _doanhNghiepRepo.KiemTraTonTaiBangIdAsync(dn_id);

        if (!tonTaiDoanhNghiep)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        if (nhaMay.NM_DN_Id == dn_id)
        {
            throw new Exception("Nhà máy đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new SuaNhaMayRequirement());

        if (checkAuth.Succeeded)
        {
            nhaMay.NM_DN_Id = dn_id;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm doanh nghiệp vào nhà máy này");
        }
    }

    public async Task XoaDoanhNghiepKhoiNhaMayAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new SuaNhaMayRequirement());

        if (checkAuth.Succeeded)
        {
            nhaMay.NM_DN_Id = null;
            nhaMay.NM_NgaySua = DateTime.Now;
            nhaMay.NM_NguoiSua_Id = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int result = await _nhaMayRepo.SuaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp của nhà máy này");
        }
    }

    public async Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new XoaNhaMayRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _nhaMayRepo.XoaAsync(nhaMay);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa nhà máy thất bại");
            }
            await _nhaMayRepo.XoaPhanQuyenNhaMayAsync(id);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa nhà máy này");
        }
    }

    public async Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangMaNhaMayAsync(nm_MaNM);
        if (nhaMay == null)
        {
            throw new Exception("Không tìm thấy nhà máy");
        }

        return nhaMay;
    }

    public async Task PhanQuyenNhaMayAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt)
    {
        var nhaMay = await _nhaMayRepo.LayMotBangIdAsync(id);

        if (nhaMay == null) throw new Exception("Sản phẩm không tồn tại");

        var userDuocPhanQuyen = await _userManager.FindByIdAsync(phanQuyenDTO.UserId.ToString());

        if (userDuocPhanQuyen == null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }

        var roleUserDuocPhanQuyen = (await _userManager.GetRolesAsync(userDuocPhanQuyen))[0];

        if (roleUserDuocPhanQuyen == Roles.ADMIN)
        {
            throw new Exception("Không thể phân quyền cho Admin");
        }

        var quyenAdminSP = await _authorizationService.AuthorizeAsync(userNowFromJwt, nhaMay, new ToanQuyenNhaMayRequirement());

        if (quyenAdminSP.Succeeded)
        {
            List<Claim> claimPhanQuyens = new List<Claim>();
            if (phanQuyenDTO.Admin)
            {
                claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Admin, id)));
            }
            else
            {
                if (phanQuyenDTO.Sua)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Sua, id)));
                }

                if (phanQuyenDTO.Xoa)
                {
                    claimPhanQuyens.Add(new Claim(AppPermissions.Permissions, AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Xoa, id)));
                }
            }
            await _nhaMayRepo.XoaPhanQuyenNhaMayAsync(id, userDuocPhanQuyen.Id);
            await _userManager.AddClaimsAsync(userDuocPhanQuyen, claimPhanQuyens);
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền phân quyền nhà máy");
        }
    }
}

