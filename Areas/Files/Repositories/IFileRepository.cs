using App.Areas.Files.Models;

namespace App.Areas.Files.Repositories;

public interface IFileRepository
{
    public Task<int> ThemMotAsync(FileModel fileModel);

    public Task<int> ThemNhieuAsync(List<FileModel> listFileModels);

    public Task<FileModel> LayMotBangIdAsync(Guid id);

    public Task<List<FileModel>> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0, bool descending = false);

    public Task<int> XoaMotAsync(FileModel fileModel);

    public Task<int> XoaNhieuAsync(List<FileModel> listFileModels);
}