using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Models;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Areas.DoanhNghiep.Auth;

public class DoanhNghiepAuthorizationHandler : IAuthorizationHandler
{
    private readonly UserManager<AppUser> _userManager;

    public DoanhNghiepAuthorizationHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is ToanQuyenDoanhNghiepRequirement)
            {
                if (await QuyenDoanhNghiepAsync(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is SuaDoanhNghiepRequirement)
            {
                if (await QuyenDoanhNghiepAsync(context.User, context.Resource, true, false))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaDoanhNghiepRequirement)
            {
                if (await QuyenDoanhNghiepAsync(context.User, context.Resource, false, true))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> QuyenDoanhNghiepAsync(ClaimsPrincipal userNowFromJwt, object resource, bool quyenSua = false, bool quyenXoa = false)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userNow = await _userManager.GetUserAsync(userNowFromJwt);

            if (userNow == null) return false;

            var doanhNghiep = resource as DoanhNghiepModel;

            var claims = await _userManager.GetClaimsAsync(userNow);

            foreach (var claim in claims)
            {
                if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Admin, doanhNghiep.DN_Id))
                {
                    return true;
                }

                if (quyenSua)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Sua, doanhNghiep.DN_Id))
                    {
                        return true;
                    }
                }

                if (quyenXoa)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.DN_Xoa, doanhNghiep.DN_Id))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

