using App.Areas.IndividualEnterprises.Model;
using App.Database;

namespace App.Areas.IndividualEnterprises.Repositories;

public interface IIndividualEnterpiseRepository : IBaseRepository<IndividualEnterpriseModel>
{
    public Task<IndividualEnterpriseModel> GetMyOneAsync(string userId);
    public Task<IndividualEnterpriseModel> GetOneByIdAsync(string id);
    public Task<IndividualEnterpriseModel> GetOneByIndividualEnterpriseCodeAsync(string individualEnterpiseCode);
    public Task<bool> CheckUserHadIndividualEnterpiseBeforeAsync(string userId);
    public Task<bool> CheckExistByTaxCodeAndGLNCodeAsync(string taxCode, string gLNCode);
    public Task<bool> CheckExistExceptThisByTaxCodeAndGLNCodeAsync(string id, string taxCode, string gLNCode);
    public Task<bool> CheckExistByIndividualEnterpriseCodeAsync(string individualEnterpiseCode);
    public Task<bool> CheckExistExceptThisByIndividualEnterpriseCodeAsync(string id, string individualEnterpiseCode);
}