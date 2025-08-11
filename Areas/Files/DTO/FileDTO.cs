using System.ComponentModel.DataAnnotations;
using Areas.Auth.DTO;

namespace App.Areas.Files.DTO;

public class FileDTO
{
    public FileDTO(string fileType, string entityType, string entityId)
    {
        FileType = fileType;
        EntityType = entityType;
        EntityId = entityId;
    }

    public Guid? Id { set; get; }
    public string? FileName { set; get; }
    public string FileType { set; get; }

    public long? Size { set; get; }
    public string EntityType { set; get; }
    public string EntityId { set; get; }
    public Guid? CreatedUserId { set; get; }
    public DateTime? CreatedAt { set; get; }
    public UserDTO? CreatedUser { set; get; }
}