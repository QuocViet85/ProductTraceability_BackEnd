using App.Areas.Batches.Models;
using App.Database;

namespace App.Areas.Batches.Repositories;

public interface IBatchRepository : IBaseRepository<BatchModel>
{
    public Task<List<BatchModel>> GetManyByProductAsync(Guid productId, int pageNumber, int limit, string search);
    public Task<int> GetTotalByProductAsync(Guid productId);
    public Task<BatchModel> GetOneByBatchCodeAsync(string batchCode);
    public Task<bool> CheckExistByBatchCodeAsync(string batchCode);
    public Task<bool> CheckExistExceptThisByBatchCodeAsync(Guid id, string batchCode);
}