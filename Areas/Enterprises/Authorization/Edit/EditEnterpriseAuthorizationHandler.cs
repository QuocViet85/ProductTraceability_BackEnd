using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Models;
using App.Database;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Enterprises.Auth.Edit;

public class EditEnterpriseAuthorizationHandler : AuthorizationHandler<CanEditEnterpriseRequirement, EnterpriseModel>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditEnterpriseRequirement requirement, EnterpriseModel resource)
    {
        if (context.User.IsInRole(Roles.ADMIN))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole(Roles.ENTERPRISE))
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            bool isOwner = resource.EnterpriseUsers.Any(eu => eu.UserId == userId);

            if (isOwner)
            {
                if (resource.EnterpriseUsers.Count > 1 && requirement.Delete)
                {
                    context.Fail(new AuthorizationFailureReason(this, "Đang sở hữu doanh nghiệp cùng người khác nên không thể xóa"));
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "Không sở hữu doanh nghiệp nên không thể thao tác với doanh nghiệp"));
            }
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Không sở hữu doanh nghiệp nên không thể thao tác với doanh nghiệp"));
        }
        return Task.CompletedTask;
    }
}

