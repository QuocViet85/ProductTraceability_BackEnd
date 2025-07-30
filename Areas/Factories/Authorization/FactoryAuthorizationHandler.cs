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
                if (await CanAddEnterpriseInFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteEnterpriseInFactoryRequirement)
            {
                if (await CanDeleteEnterpriseInFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanAddOwnerToFactoryRequirement)
            {
                if (await CanAddOwnerToFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteOwnerInFactoryRequirement)
            {
                if (await CanDeleteOwnerInFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanUpdateFactoryRequirement)
            {
                if (await CanUpdateFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement is CanDeleteFactoryRequirement)
            {
                if (await CanDeleteFactory(context.User, context.Resource, requirement))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    private async Task<bool> CanAddEnterpriseInFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
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
                var enterpriseInitial = await _enterpriseRepo.GetOneAsync((Guid)factory.EnterpriseId);
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

    private async Task<bool> CanDeleteEnterpriseInFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
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
                var enterpriseInitial = await _enterpriseRepo.GetOneAsync((Guid)factory.EnterpriseId);
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

    private async Task<bool> CanAddOwnerToFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
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

            if (userIdNow == userIdAdd && factory.EnterpriseId != null)
            {
                var enterpriseInitial = await _enterpriseRepo.GetOneAsync((Guid)factory.EnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (!isUniqueOwnerEnterprise)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private async Task<bool> CanDeleteOwnerInFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (userIdNow == factory.OwnerUserId)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> CanUpdateFactory(ClaimsPrincipal userNowFromJwt, object? resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (factory.OwnerUserId != null)
            {
                bool isOwner = factory.OwnerUserId == userIdNow;

                if (!isOwner)
                {
                    return false;
                }

                return true;
            }
            else if (factory.EnterpriseId != null)
            {
                bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner((Guid)factory.EnterpriseId, userIdNow);

                if (!isOwnerEnterprise)
                {
                    return false;
                }
                
                return true;
            }
        }
        return false;
    }
    
    private async Task<bool> CanDeleteFactory(ClaimsPrincipal userNowFromJwt, object resource, IAuthorizationRequirement requirement)
    {
        if (userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            return true;
        }
        else
        {
            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var factory = resource as FactoryModel;

            if (factory.OwnerUserId != null)
            {
                //Nhà máy đang là sở hữu cá nhân
                bool isOwnerFactory = factory.OwnerUserId == userIdNow;

                if (isOwnerFactory)
                {
                    return true;
                }
            }
            else if (factory.EnterpriseId != null)
            {
                //Nhà máy đang là sở hữu doanh nghiệp
                var enterpriseInitial = await _enterpriseRepo.GetOneAsync((Guid)factory.EnterpriseId);
                bool isUniqueOwnerEnterprise = EnterpriseHelper.IsUniqueOwnerEnterprise(enterpriseInitial, userIdNow);

                if (isUniqueOwnerEnterprise)
                {
                    return true;
                }
            }
            return false;
        }
    }
}