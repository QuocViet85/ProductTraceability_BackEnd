using App.Areas.IndividualEnterprises.Model;
using App.Database;

namespace App.Areas.IndividualEnterprises.Repositories;

public interface IIndividualEnterpiseRepository : IBaseRepository<IndividualEnterpriseModel>
{
    public Task<IndividualEnterpriseModel> GetMyOneAsync(Guid userId);
    public Task<IndividualEnterpriseModel> GetOneByIndividualEnterpriseCodeAsync(string individualEnterpiseCode);
    public Task<bool> CheckExistByOwnerUserIdAsync(Guid ownerUserId);
    public Task<bool> CheckExistByTaxCodeAndGLNCodeAsync(string taxCode, string gLNCode);
    public Task<bool> CheckExistExceptThisByTaxCodeAndGLNCodeAsync(Guid id, string taxCode, string gLNCode);
    public Task<bool> CheckExistByIndividualEnterpriseCodeAsync(string individualEnterpiseCode);
    public Task<bool> CheckExistExceptThisByIndividualEnterpriseCodeAsync(Guid id, string individualEnterpiseCode);
}