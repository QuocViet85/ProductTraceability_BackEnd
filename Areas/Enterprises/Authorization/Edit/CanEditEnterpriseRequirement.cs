using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Enterprises.Auth.Edit;

public class CanEditEnterpriseRequirement : IAuthorizationRequirement
{
    public bool Delete { set; get; }

    public CanEditEnterpriseRequirement(bool delete = false)
    {
        Delete = delete;
    }
}