using App.Areas.Files.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Files.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDBContext _dbContext;

    public FileRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> CreateManyAsync(List<FileModel> listFileModels)
    {
        await _dbContext.Files.AddRangeAsync(listFileModels);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> CreateOneAsync(FileModel fileModel)
    {
        await _dbContext.Files.AddAsync(fileModel);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<FileModel>> GetManyByEntityAsync(string entityType, string entityId, string fileType = null, int limit = 0, bool descending = false)
    {
        IQueryable<FileModel> queryFileModels = _dbContext.Files.Where(f => f.EntityType == entityType && f.EntityId == entityId);

        if (descending)
        {
            queryFileModels = queryFileModels.OrderByDescending(f => f.CreatedAt);
        }
        else
        {
            queryFileModels = queryFileModels.OrderBy(f => f.CreatedAt);
        }

        if (fileType != null)
        {
            queryFileModels = queryFileModels.Where(f => f.FileType == fileType);
        }

        if (limit > 0)
        {
            queryFileModels = queryFileModels.Take(limit);
        }

        List<FileModel> listFileModels = await queryFileModels.ToListAsync();

        return listFileModels;
    }

    public async Task<FileModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.Files.Where(f => f.Id == id).Include(f => f.CreatedUser).FirstOrDefaultAsync();
    }

    public async Task<int> DeleteOneAsync(FileModel fileModel)
    {
        _dbContext.Files.Remove(fileModel);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteManyAsync(List<FileModel> listFileModels)
    {
        _dbContext.RemoveRange(listFileModels);
        return await _dbContext.SaveChangesAsync();
    }


}