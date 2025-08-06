using App.Areas.Files.DTO;

namespace App.Areas.Files.Services;

public interface IFileService
{
    //được gọi trong api của tài nguyên khác
    public Task<int> UploadAsync(List<IFormFile> listFiles, FileDTO fileDTO);
    public Task<int> DeleteOneByIdAsync(Guid id);
    public Task<int> DeleteAllByEntityAsync(string entityType, string entityId);

    //được gọi trong api của file
    public Task<List<FileDTO>> GetFilesByEntityAsync(string entityType, string entityId, string fileType, int limit);
    public Task<FileDTO> GetOneByIdAsync(Guid id);
    
    
}

/*
- api upload file và delete file phải ở tài nguyên khác rồi gọi sang đây vì cần phân quyền.
- có api lấy file theo tài nguyên vì việc này không cần phân quyền.

Dù có hàm được gọi từ api tài nguyên khác nhưng đầu vào vẫn là DTO vì:
- Khớp với đầu ra.
- Thiết lập dữ liệu đầu vào bắt buộc = constructor có tham số bắt buộc của DTO.
*/