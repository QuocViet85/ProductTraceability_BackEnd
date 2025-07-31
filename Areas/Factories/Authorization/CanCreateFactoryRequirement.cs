using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanCreateFactoryRequirement : IAuthorizationRequirement
{
    public bool IndividualEnterpriseOwner;
    public Guid? EnterpriseId;
    public CanCreateFactoryRequirement(bool individualEnterpriseOwner, Guid? enterpriseId = null)
    {
        IndividualEnterpriseOwner = individualEnterpriseOwner;
        EnterpriseId = enterpriseId;
    }
}