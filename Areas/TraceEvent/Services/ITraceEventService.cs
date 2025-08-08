using App.Areas.TraceEvents.DTO;
using App.Services;

namespace App.Areas.TraceEvents.Services;

public interface ITraceEventService : IBaseService<TraceEventDTO>
{
    public Task<(int totalItems, List<TraceEventDTO> listDTOs)> GetManyByBatchAsync(Guid batchId, int pageNumber, int limit, string search, bool descending);
    public Task<TraceEventDTO> GetOneByTraceEventCodeAsync(string traceEventCode);
}