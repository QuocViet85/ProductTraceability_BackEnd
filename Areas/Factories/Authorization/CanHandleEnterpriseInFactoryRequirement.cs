using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanHandleEnterpriseInFactoryRequirement : IAuthorizationRequirement
{
    public Guid EnterpriseId { set; get; }
    public CanHandleEnterpriseInFactoryRequirement(Guid enterpriseId)
    {
        EnterpriseId = enterpriseId;
    }
}