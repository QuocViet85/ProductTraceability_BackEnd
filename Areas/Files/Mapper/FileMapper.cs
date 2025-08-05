using App.Areas.Files.DTO;
using App.Areas.Files.Models;

namespace App.Areas.Files.Mapper;

public static class FileMapper
{
    public static FileDTO ModelToDto(FileModel fileModel)
    {
        return new FileDTO(fileModel.FileType, fileModel.EntityType, fileModel.EntityId)
        {
            Id = fileModel.Id,
            FileName = fileModel.FileName,
            Size = fileModel.Size,
            CreatedAt = fileModel.CreatedAt
        };
    }

    public static FileModel DtoToModel(FileDTO fileDTO)
    {
        return new FileModel()
        {
            FileType = fileDTO.FileType,
            EntityId = fileDTO.EntityId,
            EntityType = fileDTO.EntityType,
            CreatedUserId = fileDTO.CreatedUserId,
        };
    }
}
