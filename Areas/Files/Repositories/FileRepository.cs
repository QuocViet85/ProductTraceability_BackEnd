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
    public async Task<int> ThemNhieuAsync(List<FileModel> listFileModels)
    {
        await _dbContext.Files.AddRangeAsync(listFileModels);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> ThemMotAsync(FileModel fileModel)
    {
        await _dbContext.Files.AddAsync(fileModel);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<FileModel>> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, string kieuFile = null, int limit = 0, bool descending = false)
    {
        IQueryable<FileModel> queryFileModels = _dbContext.Files.Where(f => f.F_KieuTaiNguyen == kieuTaiNguyen && f.F_TaiNguyenId == taiNguyenId);

        if (descending)
        {
            queryFileModels = queryFileModels.OrderByDescending(f => f.F_NgayTao);
        }
        else
        {
            queryFileModels = queryFileModels.OrderBy(f => f.F_NgayTao);
        }

        if (kieuFile != null)
        {
            queryFileModels = queryFileModels.Where(f => f.F_KieuFile == kieuFile);
        }

        if (limit > 0)
        {
            queryFileModels = queryFileModels.Take(limit);
        }

        List<FileModel> listFileModels = await queryFileModels.ToListAsync();

        return listFileModels;
    }

    public async Task<FileModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.Files.Where(f => f.F_Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> XoaMotAsync(FileModel fileModel)
    {
        _dbContext.Files.Remove(fileModel);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaNhieuAsync(List<FileModel> listFileModels)
    {
        _dbContext.RemoveRange(listFileModels);
        return await _dbContext.SaveChangesAsync();
    }


}