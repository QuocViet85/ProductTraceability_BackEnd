using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanAddEnterpriseInFactoryRequirement : IAuthorizationRequirement
{
    public Guid EnterpriseId { set; get; }
    public CanAddEnterpriseInFactoryRequirement(Guid enterpriseId)
    {
        EnterpriseId = enterpriseId;
    }
}