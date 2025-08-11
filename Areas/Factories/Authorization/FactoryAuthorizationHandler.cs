using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Repositories;
using App.Areas.Enterprises.Services;
using App.Areas.Factories.Models;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class FactoryAuthorizationHandler : IAuthorizationHandler
{
    private readonly IEnterpriseRepository _enterpriseRepo;

    public FactoryAuthorizationHandler(IEnterpriseRepository enterpriseRepo)
    {
        _enterpriseRepo = enterpriseRepo;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirements = context.Requirements;

        foreach (var requirement in requirements)
        {
            if (requirement is CanAddEnterpriseInFactoryRequirement)
            {
                if (await CanAddEnterpriseInFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteEnterpriseInFactoryRequirement)
            {
                if (await CanDeleteEnterpriseInFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanAddIndividualEnterpriseToFactoryRequirement)
            {
                if (await CanAddIndividualEnterpriseToFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteIndividualEnterpriseInFactoryRequirement)
            {
                if (await CanDeleteIndividualEnterpriseInFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanUpdateFactoryRequirement)
            {
                if (await CanUpdateFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteFactoryRequirement)
            {
                if (await CanDeleteFactoryAsync(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> CanAddEnterpriseInFactoryAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;
            var canHandleEnterpriseInFactoryRequirement = requirement as CanAddEnterpriseInFactoryRequirement;
            var enterpriseId = canHandleEnterpriseInFactoryRequirement.EnterpriseId;

            if (factory.IndividualEnterpriseId != null)
            {
                //Nhà máy đang là sở hữu hộ kinh doanh cá nhân, muốn chuyển thành sở hữu doanh nghiệp
                bool isIndividualEnterpriseOfFactory = factory.IndividualEnterpriseId.ToString() == userIdNow;

                if (!isIndividualEnterpriseOfFactory)
                {
                    return false;
                }
            }
            else if (factory.EnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp, muốn đổi sở hữu doanh nghiệp
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)factory.EnterpriseId);
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

            bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner(enterpriseId, Guid.Parse(userIdNow));

            if (!isOwnerEnterprise)
            {
                return false;
            }

            return true;
        }
        return false;
    }

    private async Task<bool> CanDeleteEnterpriseInFactoryAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;
            if (factory.EnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)factory.EnterpriseId);
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

    private async Task<bool> CanAddIndividualEnterpriseToFactoryAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;
            var canAddIndividualEnterpriseToFactoryRequirement = requirement as CanAddIndividualEnterpriseToFactoryRequirement;
            var individualEnterpriseIdAdd = canAddIndividualEnterpriseToFactoryRequirement.IndividualEnterpriseId;

            if (factory.EnterpriseId != null)
            {
                bool isOwnerOfIndividualEnterprise = individualEnterpriseIdAdd == userIdNow;

                if (!isOwnerOfIndividualEnterprise)
                {
                    return false;
                }

                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)factory.EnterpriseId);
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

    private async Task<bool> CanDeleteIndividualEnterpriseInFactoryAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (userIdNow == factory.IndividualEnterpriseId.ToString())
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> CanUpdateFactoryAsync(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (factory.IndividualEnterpriseId != null)
            {
                bool isIndividualEnterpriseOfFactory = factory.IndividualEnterpriseId.ToString() == userIdNow;

                if (isIndividualEnterpriseOfFactory)
                {
                    return true;
                }
            }
            else if (factory.EnterpriseId != null)
            {
                bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner((Guid)factory.EnterpriseId, Guid.Parse(userIdNow));

                if (isOwnerEnterprise)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> CanDeleteFactoryAsync(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else if (userNowFromJwt.IsInRole(Roles.ENTERPRISE))
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (factory.IndividualEnterpriseId != null)
            {
                //Nhà máy đang là sở hữu của hộ kinh doanh cá nhân
                bool isIndividualEnterpriseOfFactory = factory.IndividualEnterpriseId.ToString() == userIdNow;

                if (isIndividualEnterpriseOfFactory)
                {
                    return true;
                }
            }
            else if (factory.EnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var enterpriseInitial = await _enterpriseRepo.GetOneByIdAsync((Guid)factory.EnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (isUniqueOwnerEnterprise)
                {
                    return true;
                }
            }
        }
        return false;
    }
}