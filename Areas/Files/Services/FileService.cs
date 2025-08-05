using App.Areas.Auth.Mapper;
using App.Areas.Files.DTO;
using App.Areas.Files.Mapper;
using App.Areas.Files.Models;
using App.Database;
using Areas.Auth.DTO;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Files.Services;

public class FileService : IFileService
{
    private readonly AppDBContext _dbContext;
    private readonly IWebHostEnvironment _env;

    public FileService(AppDBContext dbContext, IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
    }

    public async Task<int> UploadAsync(List<IFormFile> listFiles, FileDTO fileDTO)
    {
        ValidateFiles(listFiles);

        foreach (var file in listFiles)
        {
            if (fileDTO.FileType == FileInformation.FileType.AVATAR)
            {
                await DeleteOldAvatar(fileDTO.EntityType, fileDTO.EntityId);
            }

            var fileName = GenerateFileName(Path.GetExtension(file.FileName));
            var filePath = GetFilePath(fileName, fileDTO.EntityType);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileModel = FileMapper.DtoToModel(fileDTO);
            fileModel.FileName = fileName;
            fileModel.Size = file.Length;
            fileModel.CreatedAt = DateTime.Now;

            _dbContext.Files.Add(fileModel);
        }

        return await _dbContext.SaveChangesAsync();
    }
    public async Task<List<FileDTO>> GetAllByEntityAsync(string entityType, string entityId)
    {
        IQueryable<FileModel> queryFiles = _dbContext.Files.Where(f => f.EntityType == entityType && f.EntityId == entityId);
        int totalFilesByEntity = await queryFiles.CountAsync();
        List<FileModel> listFileModels = await queryFiles.ToListAsync();

        List<FileDTO> listFileDTOs = new List<FileDTO>();
        foreach (var fileModel in listFileModels)
        {
            var fileDTO = FileMapper.ModelToDto(fileModel);
            AddRelationToDTO(fileDTO, fileModel);
            listFileDTOs.Add(fileDTO);
        }

        return listFileDTOs;
    }

    public async Task<FileDTO> GetOneByEntityAsync(string entityType, string entityId, string fileType)
    {
        var fileModel = await _dbContext.Files.Where(f => f.EntityType == entityType && f.EntityId == entityId && f.FileType == fileType).FirstOrDefaultAsync();

        if (fileModel != null)
        {
            var fileDTO = FileMapper.ModelToDto(fileModel);
            return fileDTO;
        }
        return null;
    }

    public async Task DeleteAllByEntityAsync(string entityType, string entityId)
    {
        List<FileModel> listFileModels = await _dbContext.Files.Where(f => f.EntityType == entityType && f.EntityId == entityId).ToListAsync();

        if (listFileModels.Count > 0)
        {
            foreach (var fileModel in listFileModels)
            {
                var filePath = GetFilePath(fileModel.FileName, fileModel.FileType);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            _dbContext.Files.RemoveRange(listFileModels);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<FileDTO> GetOneByIdAsync(Guid id)
    {
        var fileModel = await _dbContext.Files.Where(f => f.Id == id).FirstOrDefaultAsync();

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }
        var fileDTO = FileMapper.ModelToDto(fileModel);
        AddRelationToDTO(fileDTO, fileModel);
        return fileDTO;
    }

    public async Task<int> DeleteOneByIdAsync(Guid id)
    {
        var fileModel = await _dbContext.Files.Where(f => f.Id == id).FirstOrDefaultAsync();

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }

        var filePath = GetFilePath(fileModel.FileName, fileModel.FileType);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _dbContext.Files.Remove(fileModel);
        return await _dbContext.SaveChangesAsync();
    }
    private async Task DeleteOldAvatar(string entityType, string entityId)
    {
        var oldAvatar = await _dbContext.Files.Where(f => f.EntityId == entityId && f.EntityType == entityType && f.FileType == FileInformation.FileType.AVATAR).FirstOrDefaultAsync();

        if (oldAvatar != null)
        {
            string pathOldAvatar = GetFilePath(oldAvatar.FileName, oldAvatar.FileType);

            if (File.Exists(pathOldAvatar))
            {
                File.Delete(pathOldAvatar);
            }

            _dbContext.Files.Remove(oldAvatar);
            await _dbContext.SaveChangesAsync();
        }
    }

    private string GenerateFileName(string extension = null)
    {
        var random = new Random();

        return DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + random.Next(0, 100) + extension;
    }

    private string GetFilePath(string fileName, string fileType)
    {
        if (fileType == FileInformation.FileType.IMAGE || fileType == FileInformation.FileType.AVATAR)
        {
            return Path.Combine(_env.WebRootPath, "images", fileName);
        }
        return null;
    }

    private void ValidateFiles(List<IFormFile> listFiles)
    {
        foreach (var file in listFiles)
        {
            if (file.Length == 0)
            {
                throw new Exception($"Kích thước File: {file.FileName} không hợp lệ");
            }

            if (file.Length > FileInformation.MAX_SIZE)
            {
                throw new Exception($"Kích thước File: {file.FileName} quá lớn");
            }

            var extensionFile = Path.GetExtension(file.FileName);

            if (!FileInformation.FILE_EXTENSIONS.Contains(extensionFile))
            {
                throw new Exception($"Đuôi File: {file.FileName} không hợp lệ, chỉ được Upload File có đuôi: " + FileInformation.GetFileExtensions());
            }
        }
    }

    private void AddRelationToDTO(FileDTO fileDTO, FileModel fileModel)
    {
        if (fileModel.CreatedUser != null)
        {
            fileDTO.CreatedUser = UserMapper.ModelToDto(fileModel.CreatedUser);
        }
    }
}