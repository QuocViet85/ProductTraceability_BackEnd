using App.Areas.Batches.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Batches.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly AppDBContext _dbContext;

    public BatchRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BatchModel>> GetManyByProductAsync(Guid productId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<BatchModel> queryBatches = _dbContext.Batches.Where(c => c.ProductId == productId);

        if (descending)
        {
            queryBatches = queryBatches.OrderByDescending(b => b.ManufactureDate);
        }
        else
        {
            queryBatches = queryBatches.OrderBy(b => b.ManufactureDate);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryBatches = queryBatches.Where(b => b.Name.Contains(search) || b.BatchCode.Contains(search));
        }

        List<BatchModel> listBatches = await queryBatches.Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();

        return listBatches;
    }

    public async Task<int> GetTotalByProductAsync(Guid productId)
    {
        return await _dbContext.Batches.Where(c => c.ProductId == productId).CountAsync();
    }

    public async Task<BatchModel> GetOneByIdAsync(Guid id)
    {
        IQueryable<BatchModel> queryBatch = _dbContext.Batches.Where(c => c.Id == id);
        queryBatch = IncludeOfBatch(queryBatch);
        return await queryBatch.FirstOrDefaultAsync();
    }

    public async Task<BatchModel> GetOneByBatchCodeAsync(string batchCode)
    {
        IQueryable<BatchModel> queryBatch = _dbContext.Batches.Where(c => c.BatchCode == batchCode);
        queryBatch = IncludeOfBatch(queryBatch);
        return await queryBatch.FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(BatchModel batch)
    {
        await _dbContext.Batches.AddAsync(batch);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(BatchModel batch)
    {
        _dbContext.Batches.Remove(batch);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(BatchModel batch)
    {
        _dbContext.Batches.Update(batch);
        return await _dbContext.SaveChangesAsync();
    }

    private IQueryable<BatchModel> IncludeOfBatch(IQueryable<BatchModel> queryBatch)
    {
        return queryBatch.Include(b => b.Product)
                            .Include(b => b.CreatedUser)
                            .Include(b => b.UpdatedUser)
                            .Include(b => b.Factory);
    }

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.Batches.AnyAsync(f => f.Id == id);
    }

    public async Task<bool> CheckExistByBatchCodeAsync(string batchCode)
    {
        return await _dbContext.Batches.AnyAsync(f => f.BatchCode == batchCode);
    }

    public async Task<bool> CheckExistExceptThisByBatchCodeAsync(Guid id, string batchCode)
    {
        return await _dbContext.Batches.AnyAsync(f => f.Id != id && f.BatchCode == batchCode);
    }

    //Not Implement
    public Task<int> GetTotalAsync()
    {
        throw new NotImplementedException();
    }
    public Task<List<BatchModel>> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<List<BatchModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetMyTotalAsync(string userId)
    {
        throw new NotImplementedException();
    }
}