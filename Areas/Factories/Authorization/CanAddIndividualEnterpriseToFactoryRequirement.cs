using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanAddIndividualEnterpriseToFactoryRequirement : IAuthorizationRequirement
{
    public string IndividualEnterpriseId { set; get; }
    public CanAddIndividualEnterpriseToFactoryRequirement(string individualEnterpriseId)
    {
        IndividualEnterpriseId = individualEnterpriseId;
    }
}