using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
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
                if (await CanAddOwnerEnterpriseOfProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is XoaDoanhNghiepSoHuuSanPhamRequirement)
            {
                if (await CanDeleteOwnerEnterpriseOfProductAsync(context.User, context.Resource, requirement))
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
            var product = resource as ProductModel;

            if (product == null)
            {
                return false;
            }

            if (product.OwnerIndividualEnterpriseId != null)
            {
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId.ToString() == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                bool isOwnerEnterprise = await _doanhNghiepRepo.CheckIsOwner((Guid)product.OwnerEnterpriseId, Guid.Parse(userIdNow));

                if (isOwnerEnterprise)
                {
                    return true;
                }
            }

            if (product.ResponsibleUserId != null)
            {
                var canUpdateProductRequirement = requirement as CanUpdateProductRequirement;
                var traceCodeUpdate = canUpdateProductRequirement.TraceCode;

                if (product.TraceCode != traceCodeUpdate)
                {
                    //Không cho phép người phụ trách sửa TraceCode
                    return false;
                }

                bool isReponsibleUserOfProduct = product.ResponsibleUserId.ToString() == userIdNow;

                if (isReponsibleUserOfProduct)
                {
                    return true;
                }
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
            var product = resource as ProductModel;

            if (product.OwnerIndividualEnterpriseId != null)
            {
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId.ToString() == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                bool isOwnerEnterprise = await _doanhNghiepRepo.CheckIsOwner((Guid)product.OwnerEnterpriseId, Guid.Parse(userIdNow));

                if (isOwnerEnterprise)
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
            var product = resource as ProductModel;

            if (product.OwnerIndividualEnterpriseId != null)
            {
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId.ToString() == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                var enterpriseInitial = await _doanhNghiepRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (isUniqueOwnerEnterprise)
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
            var product = resource as ProductModel;
            var canHandleEnterpriseInFactoryRequirement = requirement as CanAddOwnerEnterpriseOfProductRequirement;
            var enterpriseId = canHandleEnterpriseInFactoryRequirement.EnterpriseId;

            if (product.OwnerIndividualEnterpriseId != null)
            {
                bool isIndividualEnterpriseOfFactory = product.OwnerIndividualEnterpriseId.ToString() == userIdNow;

                if (!isIndividualEnterpriseOfFactory)
                {
                    return false;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                var enterpriseInitial = await _doanhNghiepRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (!isUniqueOwnerEnterprise)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            bool isOwnerEnterprise = await _doanhNghiepRepo.CheckIsOwner(enterpriseId, Guid.Parse(userIdNow));

            if (!isOwnerEnterprise)
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
            var product = resource as ProductModel;
            if (product.OwnerEnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var enterpriseInitial = await _doanhNghiepRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (!isUniqueOwnerEnterprise)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }
}