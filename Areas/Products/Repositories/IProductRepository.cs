using App.Areas.Products.Models;
using App.Database;

namespace App.Areas.Products.Repositories;

public interface IProductRepository : IBaseRepository<ProductModel>
{
    public Task<ProductModel> GetOneByTraceCodeAsync(string traceCode);

    public Task<bool> CheckExistByTraceCode(string traceCode);

    public Task<bool> CheckExistExceptThisByTraceCode(Guid id, string traceCode);

    public Task<bool> CheckExistByBarCode(string barCode);

    public Task<bool> CheckExistExceptThisByBarCode(Guid id, string barCode);

    public Task<List<ProductModel>> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByCategoryAsync(Guid categoryId);

    public Task<List<ProductModel>> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByOwnerIndividualEnterpriseAsync(string individualEnterpriseId);

    public Task<List<ProductModel>> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByOwnerEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByCarrierEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByProducerEnterpriseAsync(Guid enterpriseId);

    public Task<List<ProductModel>> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByResponsibleUserAsync(string userId);

    public Task<List<ProductModel>> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalByFactoryAsync(Guid factoryId);
}


/*
Có api lấy Product từ tài nguyên khác vì:
- Product trong các tài nguyên khác có thể rất nhiều nên cần limit, search. 
- Người dùng cần lấy product từ 1 tài nguyên khác. 

*/