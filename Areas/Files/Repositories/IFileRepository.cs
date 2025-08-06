using App.Areas.Files.Models;

namespace App.Areas.Files.Repositories;

public interface IFileRepository
{
    public Task<int> CreateOneAsync(FileModel fileModel);

    public Task<int> CreateManyAsync(List<FileModel> listFileModels);

    public Task<FileModel> GetOneByIdAsync(Guid id);

    public Task<List<FileModel>> GetFilesByEntityAsync(string entityType, string entityId, string fileType = null, int limit = 0);

    public Task<int> DeleteOneAsync(FileModel fileModel);

    public Task<int> DeleteManyAsync(List<FileModel> listFileModels);
}