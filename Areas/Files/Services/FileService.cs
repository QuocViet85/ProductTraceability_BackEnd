using App.Areas.Auth.Mapper;
using App.Areas.Files.DTO;
using App.Areas.Files.Mapper;
using App.Areas.Files.Models;
using App.Areas.Files.Repositories;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Files.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IFileRepository _fileRepo;

    public FileService(IWebHostEnvironment env, IFileRepository fileRepo)
    {
        _fileRepo = fileRepo;
        _env = env;
    }

    //được gọi trong api của tài nguyên khác
    public async Task<int> UploadAsync(List<IFormFile> listFiles, FileDTO fileDTO)
    {
        ValidateFiles(listFiles);

        List<FileModel> listFileModels = new List<FileModel>();

        foreach (var file in listFiles)
        {
            var fileName = GenerateFileName(Path.GetExtension(file.FileName));
            var filePath = GetFilePath(fileName, fileDTO.FileType);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileModel = FileMapper.DtoToModel(fileDTO);
            fileModel.FileName = fileName;
            fileModel.Size = file.Length;
            fileModel.CreatedAt = DateTime.Now;

            listFileModels.Add(fileModel);
        }

        return await _fileRepo.CreateManyAsync(listFileModels);
    }


    public async Task<int> DeleteManyByEntityAsync(string entityType, string entityId, string fileType = null, int limit = 0)
    {
        List<FileModel> listFileModels = await _fileRepo.GetManyByEntityAsync(entityType, entityId, fileType, limit);

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
            return await _fileRepo.DeleteManyAsync(listFileModels);
        }
        else
        {
            return 0;
        }
    }

    public async Task<int> DeleteOneByIdAsync(Guid id)
    {
        var fileModel = await _fileRepo.GetOneByIdAsync(id);

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }

        var filePath = GetFilePath(fileModel.FileName, fileModel.FileType);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return await _fileRepo.DeleteOneAsync(fileModel);
    }

    //được gọi trong api của file
    public async Task<List<FileDTO>> GetManyByEntityAsync(string entityType, string entityId, string fileType = null, int limit = 0)
    {
        List<FileModel> listFileModels = await _fileRepo.GetManyByEntityAsync(entityType, entityId, fileType, limit);

        List<FileDTO> listFileDTOs = new List<FileDTO>();
        foreach (var fileModel in listFileModels)
        {
            var fileDTO = FileMapper.ModelToDto(fileModel);
            AddRelationToDTO(fileDTO, fileModel);
            listFileDTOs.Add(fileDTO);
        }

        return listFileDTOs;
    }

    public async Task<FileDTO> GetOneByIdAsync(Guid id)
    {
        var fileModel = await _fileRepo.GetOneByIdAsync(id);

        if (fileModel == null)
        {
            throw new Exception("Không tìm thấy file");
        }
        var fileDTO = FileMapper.ModelToDto(fileModel);
        AddRelationToDTO(fileDTO, fileModel);
        return fileDTO;
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
                throw new Exception($"Đuôi File: {file.FileName} không hợp lệ, chỉ được Upload File có đuôi: " + FileInformation.GetFileExtensionsAllowed());
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