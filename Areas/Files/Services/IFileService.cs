using App.Areas.Files.DTO;

namespace App.Areas.Files.Services;

public interface IFileService
{
    public Task<FileDTO> GetOneByIdAsync(Guid id);
    public Task<int> UploadAsync(List<IFormFile> listFiles, FileDTO fileDTO);
    public Task<FileDTO> GetOneByEntityAsync(string entityType, string entityId, string fileType);
    public Task<List<FileDTO>> GetAllByEntityAsync(string entityType, string entityId);
    public Task<int> DeleteOneByIdAsync(Guid id);
    public Task DeleteAllByEntityAsync(string entityType, string entityId);
}