using App.Areas.Products.Models;
using App.Database;

namespace App.Areas.Products.Repositories;

public interface IProductRepository : IBaseRepository<ProductModel>
{
    public Task<ProductModel> GetOneByTraceCodeAsync(string traceCode);

    public Task<bool> CheckExistByTraceCode(string traceCode);

    public Task<bool> CheckExistExceptThisByTraceCode(Guid id, string traceCode);

    public Task<List<ProductModel>> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByCategoryAsync(Guid categoryId);

    public Task<List<ProductModel>> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByOwnerIndividualEnterpriseAsync(string individualEnterpriseId);

    public Task<List<ProductModel>> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByOwnerEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByCarrierEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByProducerEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByResponsibleUserAsync(string userId);

    public Task<List<ProductModel>> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search);

    public Task<int> GetTotalByFactoryAsync(Guid factoryId);
}
