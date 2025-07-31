using App.Areas.IndividualEnterprises.Model;
using App.Database;

namespace App.Areas.IndividualEnterprises.Repositories;

public interface IIndividualEnterpiseRepository : IBaseRepository<IndividualEnterpriseModel>
{
    public Task<IndividualEnterpriseModel> GetMyOneAsync(string userId);
    public Task<IndividualEnterpriseModel> GetOneAsync(string id);
    public Task<bool> CheckUserHadIndividualEnterpiseBeforeAsync(string userId);
    public Task<bool> CheckExistByCodeAsync(string taxCode, string gLNCode);

    public Task<bool> CheckExistExceptThisByCodeAsync(string id, string taxCode, string gLNCode);
}