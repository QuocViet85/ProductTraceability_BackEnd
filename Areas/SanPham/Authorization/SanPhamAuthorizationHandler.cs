using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Models;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.SanPham.Models;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Areas.SanPham.Authorization;

public class SanPhamAuthorizationHandler : IAuthorizationHandler
{
    private readonly UserManager<AppUser> _userManager;

    public SanPhamAuthorizationHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is ToanQuyenSanPhamRequirement)
            {
                if (await QuyenSanPhamAsync(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is ThemSanPhamRequirement)
            {
                if (await QuyenSanPhamAsync(context.User, context.Resource, true))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is SuaSanPhamRequirement)
            {
                if (await QuyenSanPhamAsync(context.User, context.Resource, false, true))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaSanPhamRequirement)
            {
                if (await QuyenSanPhamAsync(context.User, context.Resource, false, false, true))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> QuyenSanPhamAsync(ClaimsPrincipal userNowFromJwt, object resource, bool quyenThem = false, bool quyenSua = false, bool quyenXoa = false)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userNow = await _userManager.GetUserAsync(userNowFromJwt);

            if (userNow == null) return false;

            DoanhNghiepModel doanhNghiepSoHuuSanPham = null;

            if (resource is DoanhNghiepModel)
            {
                doanhNghiepSoHuuSanPham = resource as DoanhNghiepModel;
            }
            else if (resource is SanPhamModel)
            {
                var sanPham = resource as SanPhamModel;
                doanhNghiepSoHuuSanPham = sanPham.SP_DN_SoHuu;
            }
            else
            {
                return false;
            }

            if (doanhNghiepSoHuuSanPham == null)
            {
                return false;
            }

            var claims = await _userManager.GetClaimsAsync(userNow);

            foreach (var claim in claims)
            {
                if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Admin, doanhNghiepSoHuuSanPham.DN_Id))
                {
                    return true;
                }

                if (quyenThem)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Them, doanhNghiepSoHuuSanPham.DN_Id))
                    {
                        return true;
                    }
                }

                if (quyenSua)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Sua, doanhNghiepSoHuuSanPham.DN_Id))
                    {
                        return true;
                    }
                }

                if (quyenXoa)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.SP_DN_Xoa, doanhNghiepSoHuuSanPham.DN_Id))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}