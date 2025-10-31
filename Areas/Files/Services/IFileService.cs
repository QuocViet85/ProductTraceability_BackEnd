using System.Security.Claims;
using App.Areas.Files.Models;

namespace App.Areas.Files.Services;

public interface IFileService
{
    //được gọi trong api của tài nguyên khác
    public Task<int> TaiLenAsync(List<IFormFile> listFiles, string kieuFile, string kieuTaiNguyen, Guid taiNguyenId, ClaimsPrincipal userNowFromJwt);
    public Task<int> XoaMotBangIdAsync(Guid id);
    public Task<int> XoaNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0);

    //được gọi trong api của file
    public Task<List<FileModel>> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0, bool descending = false);
    public Task<FileModel> LayMotBangIdAsync(Guid id);

    //Hàm trợ giúp xử lý file, có thể được dùng bởi module khác
    public string TaoTenFile(string extension = null);
    public string LayDuongDanFile(string tenFile, string kieuFile, string kieuTaiNguyen, Guid taiNguyenId);
    public void ValidateFiles(List<IFormFile> listFiles);
}

/*
- api upload file và delete file phải ở tài nguyên khác rồi gọi sang đây vì cần phân quyền.
- có api lấy file theo tài nguyên vì việc này không cần phân quyền.

Dù có hàm được gọi từ api tài nguyên khác nhưng đầu vào vẫn là DTO vì:
- Khớp với đầu ra.
- Thiết lập dữ liệu đầu vào bắt buộc = constructor có tham số bắt buộc của DTO.
*/