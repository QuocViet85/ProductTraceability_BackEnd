using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanAddOwnerToFactoryRequirement : IAuthorizationRequirement
{
    public string UserId { set; get; }
    public CanAddOwnerToFactoryRequirement(string userId)
    {
        UserId = userId;
    }
}