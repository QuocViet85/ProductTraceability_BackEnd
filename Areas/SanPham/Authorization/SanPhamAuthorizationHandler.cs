using System.Security.Claims;
using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Helpers;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.DoanhNghiep.Services;
using App.Areas.SanPham.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.SanPham.Authorization;

public class SanPhamAuthorizationHandler : IAuthorizationHandler
{
    private readonly IDoanhNghiepRepository _doanhNghiepRepo;

    public SanPhamAuthorizationHandler(IDoanhNghiepRepository doanhNghiepRepo)
    {
        _doanhNghiepRepo = doanhNghiepRepo;
    }
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is SuaSanPhamRequirement)
            {
                if (await CoTheSuaSanPhamAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is SuaQuanHeCuaSanPhamRequirement)
            {
                if (await CoTheSuaQuanHeCuaSanPhamAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaSanPhamRequirement)
            {
                if (await CoTheXoaSanPhamAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is ThemDoanhNghiepSoHuuSanPhamRequirement)
            {
                if (await CoTheThemDoanhNghiepSoHuuSanPhamAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaDoanhNghiepSoHuuSanPhamRequirement)
            {
                if (await CoTheXoaDoanhNghiepSoHuuSanPhamAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> CoTheSuaSanPhamAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sanPham = resource as SanPhamModel;

            if (sanPham == null)
            {
                return false;
            }

            if (sanPham.SP_DN_SoHuu_Id != null)
            {
                bool laChuDNCuaSanPham = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync((Guid)sanPham.SP_DN_SoHuu_Id, Guid.Parse(userIdNow));

                if (laChuDNCuaSanPham)
                {
                    return true;
                }
            }

            if (sanPham.SP_NguoiPhuTrach_Id != null)
            {
                bool laNguoiPhuTrachSanPham = sanPham.SP_NguoiPhuTrach_Id.ToString() == userIdNow;

                if (!laNguoiPhuTrachSanPham)
                {
                    return false;
                }

                var suaSanPhamRequirement = requirement as SuaSanPhamRequirement;
                var maTruyXuatUpdate = suaSanPhamRequirement.SP_MaTruyXuat;

                if (sanPham.SP_MaTruyXuat != maTruyXuatUpdate)
                {
                    //Không cho phép người phụ trách sửa TraceCode
                    return false;
                }

                return true;
            }
        }
        return false;
    }

    private async Task<bool> CoTheSuaQuanHeCuaSanPhamAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sanPham = resource as SanPhamModel;

            if (sanPham.SP_DN_SoHuu_Id != null)
            {
                bool laChuDNCuaSanPham = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync((Guid)sanPham.SP_DN_SoHuu_Id, Guid.Parse(userIdNow));

                if (laChuDNCuaSanPham)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private async Task<bool> CoTheXoaSanPhamAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sanPham = resource as SanPhamModel;

            if (sanPham.SP_DN_SoHuu_Id != null)
            {
                var doanhNghiepCuaSanPham = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)sanPham.SP_DN_SoHuu_Id);
                bool laChuDNDuyNhatCuaSanPham = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepCuaSanPham, Guid.Parse(userIdNow));

                if (laChuDNDuyNhatCuaSanPham)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private async Task<bool> CoTheThemDoanhNghiepSoHuuSanPhamAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sanPham = resource as SanPhamModel;
            var themDoanhNghiepSoHuuSanPhamRequirement = requirement as ThemDoanhNghiepSoHuuSanPhamRequirement;
            var dn_id = themDoanhNghiepSoHuuSanPhamRequirement.DN_Id;

            if (sanPham.SP_DM_Id != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                var doanhNghiepCuaSanPham = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)sanPham.SP_DM_Id);
                bool laChuDNDuyNhatCuaSanPham = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepCuaSanPham, Guid.Parse(userIdNow));

                if (!laChuDNDuyNhatCuaSanPham)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            bool laChuDNThemVaoSanPham = await _doanhNghiepRepo.KiemTraLaChuDoanhNghiepAsync(dn_id, Guid.Parse(userIdNow));

            if (!laChuDNThemVaoSanPham)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private async Task<bool> CoTheXoaDoanhNghiepSoHuuSanPhamAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sanPham = resource as SanPhamModel;

            if (sanPham.SP_DN_SoHuu_Id != null)
            {
                var doanhNghiepCuaSanPham = await _doanhNghiepRepo.LayMotBangIdAsync((Guid)sanPham.SP_DN_SoHuu_Id);
                bool laChuDNDuyNhatCuaSanPham = DoanhNghiepHelper.LaChuDoanhNghiepDuyNhat(doanhNghiepCuaSanPham, Guid.Parse(userIdNow));

                if (laChuDNDuyNhatCuaSanPham)
                {
                    return true;
                }
            }
        }
        return false;
    }
}