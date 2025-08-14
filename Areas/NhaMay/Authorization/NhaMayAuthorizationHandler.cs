using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Helpers;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.DoanhNghiep.Services;
using App.Areas.NhaMay.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.NhaMay.Authorization;

public class NhaMayAuthorizationHandler : IAuthorizationHandler
{
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;

    public NhaMayAuthorizationHandler(IDoanhNghiepRepository doanhNghiepRepo)
    {
        _doanhNghiepRepo = doanhNghiepRepo;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is ThemDoanhNghiepVaoNhaMayRequirement)
            {
                if (await ThemDoanhNghiepVaoNhaMayAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaDoanhNghiepKhoiNhaMayRequirement)
            {
                if (await XoaDoanhNghiepKhoiNhaMayAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is SuaNhaMayRequirement)
            {
                if (await SuaNhaMayAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaNhaMayRequirement)
            {
                if (await XoaNhaMayAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> ThemDoanhNghiepVaoNhaMayAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nhaMay = resource as NhaMayModel;
            var themDoanhNghiepVaoNhaMayRequirement = requirement as ThemDoanhNghiepVaoNhaMayRequirement;
            var dn_id = themDoanhNghiepVaoNhaMayRequirement.DN_Id;

            if (nhaMay.NM_DN_Id != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                var doanhNghiepHienTai = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)nhaMay.NM_DN_Id);
                bool laChuDoanhNghiepDuyNhat = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepHienTai, Guid.Parse(userIdNow));

                if (!laChuDoanhNghiepDuyNhat)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            bool laChuDoanhNghiepMoi = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync(dn_id, Guid.Parse(userIdNow));

            if (!laChuDoanhNghiepMoi)
            {
                return false;
            }

            return true;
        }
        return false;
    }

    private async Task<bool> XoaDoanhNghiepKhoiNhaMayAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nhaMay = resource as NhaMayModel;
            if (nhaMay.NM_DN_Id != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var doanhNghiepHienTai = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)nhaMay.NM_DN_Id);
                bool laChuDoanhNghiepDuyNhat = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepHienTai, Guid.Parse(userIdNow));

                if (!laChuDoanhNghiepDuyNhat)
                {
                    return false;
                }
            }
        }
        return false;
    }


    private async Task<bool> SuaNhaMayAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nhaMay = resource as NhaMayModel;

            if (nhaMay.NM_DN_Id != null)
            {
                bool laChuDoanhNghiepCuaNhaMay = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync((Guid)nhaMay.NM_DN_Id, Guid.Parse(userIdNow));

                if (laChuDoanhNghiepCuaNhaMay)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> XoaNhaMayAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nhaMay = resource as NhaMayModel;

            if (nhaMay.NM_DN_Id != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var doanhNghiepHienTai = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)nhaMay.NM_DN_Id);
                bool laChuDoanhNghiepDuyNhat = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepHienTai, Guid.Parse(userIdNow));

                if (!laChuDoanhNghiepDuyNhat)
                {
                    return false;
                }
            }
        }
        return false;
    }
}