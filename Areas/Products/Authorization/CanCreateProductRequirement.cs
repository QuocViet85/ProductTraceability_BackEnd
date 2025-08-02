using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Authorization;

public class CanCreateProductRequirement : IAuthorizationRequirement
{
    public bool OwnerIsIndividualEnterprise { set; get; }
    public Guid? OwnerEnterpriseId { set; get; }

    public CanCreateProductRequirement(bool ownerIsIndividualEnterprise, Guid? ownerEnterpriseId = null)
    {
        OwnerIsIndividualEnterprise = ownerIsIndividualEnterprise;
        OwnerEnterpriseId = ownerEnterpriseId;
    }
}