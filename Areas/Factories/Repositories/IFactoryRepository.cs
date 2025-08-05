using App.Areas.Factories.Models;
using App.Database;

namespace App.Areas.Factories.Repositories;

public interface IFactoryRepository : IBaseRepository<FactoryModel>
{
    public Task<bool> CheckExistByFactoryCodeAsync(string factoryCode);
    public Task<bool> CheckExistExceptThisByFactoryCodeAsync(Guid id, string factoryCode);
    public Task<FactoryModel> GetOneByFactoryCodeAsync(string factoryCode);
}