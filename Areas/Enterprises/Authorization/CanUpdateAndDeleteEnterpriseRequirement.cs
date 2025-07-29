using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Enterprises.Auth;

public class CanUpdateAndDeleteEnterpriseRequirement : IAuthorizationRequirement
{
    public bool Delete { set; get; }

    public CanUpdateAndDeleteEnterpriseRequirement(bool delete = false)
    {
        Delete = delete;
    }
}