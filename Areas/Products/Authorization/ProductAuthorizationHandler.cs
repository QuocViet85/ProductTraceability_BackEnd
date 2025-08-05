using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Repositories;
using App.Areas.Enterprises.Services;
using App.Areas.IndividualEnterprises.Repositories;
using App.Areas.Products.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Authorization;

public class ProductAuthorizationHandler : IAuthorizationHandler
{
    private readonly IEnterpriseRepository _enterpriseRepo;

    public ProductAuthorizationHandler(IEnterpriseRepository enterpriseRepo)
    {
        _enterpriseRepo = enterpriseRepo;
    }
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is CanUpdateProductRequirement)
            {
                if (await CanUpdateProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanUpdateSomeRelationsProductRequirement)
            {
                if (await CanUpdateSomeRelationsOfProduct(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteProductRequirement)
            {
                if (await CanDeleteProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanAddOwnerIndividualEnterpriseOfProductRequirement)
            {
                if (await CanAddOwnerIndividualEnterpriseOfProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteOwnerIndividualEnterpriseOfProductRequirement)
            {
                if (await CanDeleteOwnerIndividualEnterpriseOfProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanAddOwnerEnterpriseOfProductRequirement)
            {
                if (await CanAddOwnerEnterpriseOfProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteOwnerEnterpriseOfProductRequirement)
            {
                if (await CanDeleteOwnerEnterpriseOfProductAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> CanUpdateProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
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
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner((Guid)product.OwnerEnterpriseId, userIdNow);

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

                bool isReponsibleUserOfProduct = product.ResponsibleUserId == userIdNow;

                if (isReponsibleUserOfProduct)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> CanUpdateSomeRelationsOfProduct(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
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
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner((Guid)product.OwnerEnterpriseId, userIdNow);

                if (isOwnerEnterprise)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private async Task<bool> CanDeleteProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
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
                bool isIndividualEnterpriseOfProduct = product.OwnerIndividualEnterpriseId == userIdNow;

                if (isIndividualEnterpriseOfProduct)
                {
                    return true;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (isUniqueOwnerEnterprise)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> CanAddOwnerIndividualEnterpriseOfProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = resource as ProductModel;
            var canAddIndividualEnterpriseToProductRequirement = requirement as CanAddOwnerIndividualEnterpriseOfProductRequirement;
            var individualEnterpriseIdAdd = canAddIndividualEnterpriseToProductRequirement.IndividualEnterpriseId;

            if (product.OwnerEnterpriseId != null)
            {
                bool isIndividualEnterprise = individualEnterpriseIdAdd == userIdNow;

                if (!isIndividualEnterprise)
                {
                    return false;
                }

                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
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

    private async Task<bool> CanDeleteOwnerIndividualEnterpriseOfProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = resource as ProductModel;

            if (userIdNow == product.OwnerIndividualEnterpriseId)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> CanAddOwnerEnterpriseOfProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
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
                bool isIndividualEnterpriseOfFactory = product.OwnerIndividualEnterpriseId == userIdNow;

                if (!isIndividualEnterpriseOfFactory)
                {
                    return false;
                }
            }
            else if (product.OwnerEnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
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

            bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner(enterpriseId, userIdNow);

            if (!isOwnerEnterprise)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private async Task<bool> CanDeleteOwnerEnterpriseOfProductAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
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
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)product.OwnerEnterpriseId);
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