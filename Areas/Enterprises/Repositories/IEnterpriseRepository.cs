using App.Areas.Enterprises.Models;
using App.Database;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace App.Areas.Enterprises.Repositories;

public interface IEnterpriseRepository : IBaseRepository<EnterpriseModel>
{
    public Task<bool> CheckExistByIdAsync(Guid id);
    
    public Task<bool> CheckExistByCodeAsync(string taxCode, string gLNCode);

    public Task<bool> CheckExistExceptThisByCodeAsync(Guid id, string taxCode, string gLNCode);

    public Task<bool> CheckIsOwner(Guid id, string userId);

    public Task<int> AddOwnershipAsync(EnterpriseUserModel enterpriseUser);

    public Task<int> GiveUpOwnershipAsync(Guid id, string userId);

    public Task<int> DeleteOwnershipAsync(Guid id, string userId);
}