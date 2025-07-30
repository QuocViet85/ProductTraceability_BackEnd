using App.Areas.IndividualEnterprises.Model;
using App.Database;

namespace App.Areas.IndividualEnterprises.Repositories;

public interface IIndivisualEnterpriseRepository : IBaseRepository<IndividualEnterpriseModel>
{
    public Task<IndividualEnterpriseModel> GetMyOneAsync(string userId);

    public Task<IndividualEnterpriseModel> GetOneAsync(string id);
}