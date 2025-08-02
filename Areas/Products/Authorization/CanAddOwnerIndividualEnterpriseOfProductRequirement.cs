using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Authorization;

public class CanAddOwnerIndividualEnterpriseOfProductRequirement : IAuthorizationRequirement
{
    public string IndividualEnterpriseId { set; get; }

    public CanAddOwnerIndividualEnterpriseOfProductRequirement(string individualEnterpriseId)
    {
        IndividualEnterpriseId = individualEnterpriseId;
    }
}