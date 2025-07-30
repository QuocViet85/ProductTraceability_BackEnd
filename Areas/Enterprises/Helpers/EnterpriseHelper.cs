using App.Areas.Enterprises.Models;

namespace App.Areas.Enterprises.Services;

public static class EnterpriseHelper
{
    public static bool IsUniqueOwnerEnterprise(EnterpriseModel enterprise, string userId)
    {
        if (enterprise == null)
        {
            return false;
        }

        bool isOwnerEnterprise = enterprise.EnterpriseUsers.Any(eu => eu.UserId == userId);

        if (!isOwnerEnterprise)
        {
            return false;
        }

        bool isUniqueOwnerEnterprise = enterprise.EnterpriseUsers.Count == 1;

        if (!isUniqueOwnerEnterprise)
        {
            return false;
        }

        return true;
    }
}