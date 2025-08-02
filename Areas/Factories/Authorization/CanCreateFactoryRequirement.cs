using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Factories.Authorization;

public class CanCreateFactoryRequirement : IAuthorizationRequirement
{
    public bool OwnerIsIndividualEnterprise;
    public Guid? EnterpriseId;
    public CanCreateFactoryRequirement(bool ownerIsIndividualEnterprise, Guid? enterpriseId = null)
    {
        OwnerIsIndividualEnterprise = ownerIsIndividualEnterprise;
        EnterpriseId = enterpriseId;
    }
}