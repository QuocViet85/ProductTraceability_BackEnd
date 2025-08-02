using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Authorization;

public class CanAddOwnerEnterpriseOfProductRequirement : IAuthorizationRequirement
{
    public Guid EnterpriseId { set; get; }

    public CanAddOwnerEnterpriseOfProductRequirement(Guid enterpriseId)
    {
        EnterpriseId = enterpriseId;
    }
}