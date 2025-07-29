using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Repositories;
using App.Areas.Enterprises.Services;
using App.Areas.Factories.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class FactoryAuthorizationHandler : IAuthorizationHandler
{
    private readonly IEnterpriseService _enterpriseService;

    private readonly IEnterpriseRepository _enterpriseRepo;

    public FactoryAuthorizationHandler(IEnterpriseService enterpriseService, IEnterpriseRepository enterpriseRepo)
    {
        _enterpriseService = enterpriseService;
        _enterpriseRepo = enterpriseRepo;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is CanHandleEnterpriseInFactoryRequirement)
            {
                if (await CanHandleEnterpriseInFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanAddOwnerToFactoryRequirement)
            {

            }
        }
    }

    public async Task<bool> CanHandleEnterpriseInFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;
            var canHandleEnterpriseInFactoryRequirement = requirement as CanHandleEnterpriseInFactoryRequirement;
            var enterpriseId = canHandleEnterpriseInFactoryRequirement.EnterpriseId;

            if (factory.OwnerUserId != null)
            {
                //Nhà máy đang là sở hữu cá nhân, muốn chuyển thành sở hữu doanh nghiệp
                bool isOwnerFactory = factory.OwnerUserId == userIdNow;

                if (!isOwnerFactory)
                {
                    return false;
                }
            }
            else if (factory.EnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                bool isUniqueOwnerEnterprise = await _enterpriseService.IsUniqueOwnerEnterprise((Guid)factory.EnterpriseId, userIdNow);

                if (!isUniqueOwnerEnterprise)
                {
                    return false;
                }
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

    public async Task<bool> CanAddOwnerToFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;
            var canAddOwnerToFactoryRequirement = requirement as CanAddOwnerToFactoryRequirement;
            var userIdAdd = canAddOwnerToFactoryRequirement.UserId;

            if (userIdNow == userIdAdd)
            {
                //
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}