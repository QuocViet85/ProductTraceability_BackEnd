using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.DoanhNghiep.Models;
using App.Database;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.DoanhNghiep.Auth;

public class DoanhNghiepAuthorizationHandler : AuthorizationHandler<SuaXoaDoanhNghiepRequirement, DoanhNghiepModel>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuaXoaDoanhNghiepRequirement requirement, DoanhNghiepModel doanhNghiep)
    {
        if (context.User.IsInRole(Roles.ADMIN))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole(Roles.ENTERPRISE))
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            bool isOwner = doanhNghiep.DN_List_CDN.Any(cdn => cdn.CDN_ChuDN_Id.ToString() == userId);

            if (isOwner)
            {
                if (doanhNghiep.DN_List_CDN.Count > 1 && requirement.Xoa)
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
            context.Fail(new AuthorizationFailureReason(this, "Tài khoản không có quyền thao tác với doanh nghiệp"));
        }
        return Task.CompletedTask;
    }
}

