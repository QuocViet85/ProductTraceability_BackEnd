using App.Areas.Batches.DTO;
using App.Services;

namespace App.Areas.Batches.Services;

public interface IBatchService : IBaseService<BatchDTO>
{
    public Task<(int totalItems, List<BatchDTO> listDTOs)> GetManyByProductAsync(Guid productId, int pageNumber, int limit, string search);

    public Task<BatchDTO> GetOneByBatchCodeAsync(string batchCode);
}