using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.NhaMay.Models;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Areas.NhaMay.Authorization;

public class NhaMayAuthorizationHandler : IAuthorizationHandler
{
    private readonly UserManager<AppUser> _userManager;

    public NhaMayAuthorizationHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is ToanQuyenNhaMayRequirement)
            {
                if (await QuyenNhaMayAsync(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is SuaNhaMayRequirement)
            {
                if (await QuyenNhaMayAsync(context.User, context.Resource, true))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaNhaMayRequirement)
            {
                if (await QuyenNhaMayAsync(context.User, context.Resource, false, true))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> QuyenNhaMayAsync(ClaimsPrincipal userNowFromJwt, object resource, bool quyenSua = false, bool quyenXoa = false)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userNow = await _userManager.GetUserAsync(userNowFromJwt);

            if (userNow == null) return false;

            var nhaMay = resource as NhaMayModel;

            var claims = await _userManager.GetClaimsAsync(userNow);

            foreach (var claim in claims)
            {
                if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Admin, nhaMay.NM_Id))
                {
                    return true;
                }

                if (quyenSua)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Sua, nhaMay.NM_Id))
                    {
                        return true;
                    }
                }

                if (quyenXoa)
                {
                    if (claim.Type == AppPermissions.Permissions && claim.Value == AppPermissions.TaoGiaTriPhanQuyen(AppPermissions.NM_Xoa, nhaMay.NM_Id))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    } 
}