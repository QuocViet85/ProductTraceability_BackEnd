using App.Areas.TraceEvents.Models;
using App.Database;

namespace App.Areas.TraceEvents.Repositories;

public interface ITraceEventRepository : IBaseRepository<TraceEventModel>
{
    public Task<List<TraceEventModel>> GetManyByBatchAsync(Guid batchId, int pageNumber, int limit, string search, bool descending);
    public Task<int> GetTotalByBatchAsync(Guid batchId);
    public Task<bool> CheckExistByTraceEventCodeAsync(string traceEventCode);
    public Task<bool> CheckExistExceptThisByTraceEventCodeAsync(Guid id, string traceEventCode);
    public Task<TraceEventModel> GetOneByTraceEventCodeAsync(string traceEventCode);
}