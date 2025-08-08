using App.Areas.Batches.Models;
using App.Areas.Batches.Repositories;
using App.Areas.TraceEvents.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.TraceEvents.Repositories;

public class TraceEventRepository : ITraceEventRepository
{
    private readonly AppDBContext _dbContext;

    public TraceEventRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TraceEventModel>> GetManyByBatchAsync(Guid batchId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<TraceEventModel> queryTraceEvents = _dbContext.TraceEvents.Where(te => te.BatchId == batchId);

        if (descending)
        {
            queryTraceEvents = queryTraceEvents.OrderByDescending(te => te.TimeStamp);
        }
        else
        {
            queryTraceEvents = queryTraceEvents.OrderBy(te => te.TimeStamp);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryTraceEvents = queryTraceEvents.Where(te => te.Name.Contains(search) || te.TraceEventCode.Contains(search) || te.Description.Contains(search));
        }

        return await queryTraceEvents.Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();
    }

    public async Task<int> GetTotalByBatchAsync(Guid batchId)
    {
        return await _dbContext.TraceEvents.Where(te => te.BatchId == batchId).CountAsync();
    }

    public async Task<TraceEventModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.TraceEvents.Where(te => te.Id == id).Include(te => te.Batch).ThenInclude(b => b.Product).Include(te => te.CreatedUser).Include(te => te.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<TraceEventModel> GetOneByTraceEventCodeAsync(string traceEventCode)
    {
        return await _dbContext.TraceEvents.Where(te => te.TraceEventCode == traceEventCode).Include(te => te.Batch).ThenInclude(b => b.Product).Include(te => te.CreatedUser).Include(te => te.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.TraceEvents.AnyAsync(te => te.Id == id);
    }

    public async Task<bool> CheckExistByTraceEventCodeAsync(string traceEventCode)
    {
        return await _dbContext.TraceEvents.AnyAsync(te => te.TraceEventCode == traceEventCode);
    }

    public async Task<bool> CheckExistExceptThisByTraceEventCodeAsync(Guid id, string traceEventCode)
    {
        return await _dbContext.TraceEvents.AnyAsync(te => te.Id != id && te.TraceEventCode == traceEventCode);
    }

    public async Task<int> CreateAsync(TraceEventModel traceEvent)
    {
        await _dbContext.TraceEvents.AddAsync(traceEvent);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(TraceEventModel traceEvent)
    {
        _dbContext.TraceEvents.Update(traceEvent);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(TraceEventModel traceEvent)
    {
        _dbContext.TraceEvents.Remove(traceEvent);
        return await _dbContext.SaveChangesAsync();
    }

    //Not Implement
    public Task<List<TraceEventModel>> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<List<TraceEventModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetMyTotalAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalAsync()
    {
        throw new NotImplementedException();
    }
}