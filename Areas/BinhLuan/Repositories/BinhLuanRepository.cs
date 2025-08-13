using System.Data.Common;
using App.Areas.BinhLuan.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.BinhLuan.Repositories;

public class BinhLuanRepository : IBinhLuanRepository
{
    private readonly AppDBContext _dbContext;

    public BinhLuanRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BinhLuanModel>> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit)
    {
        return await _dbContext.BinhLuans.Where(c => c.BL_SP_Id == sp_Id).OrderByDescending(c => c.BL_NgayTao).Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();
    }

    public async Task<int> LayTongSoBangSanPhamAsync(Guid sp_Id)
    {
        return await _dbContext.BinhLuans.Where(c => c.BL_SP_Id == sp_Id).CountAsync();
    }

    public async Task<int> ThemAsync(BinhLuanModel binhLuan)
    {
        _dbContext.BinhLuans.Add(binhLuan);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(BinhLuanModel binhLuan)
    {
        _dbContext.BinhLuans.Remove(binhLuan);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.BinhLuans.AnyAsync(c => c.Id == id);
    }

    //Not Implement

    public Task<List<BinhLuanModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> LayTongSoAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<BinhLuanModel> LayMotBangIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<BinhLuanModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> SuaAsync(BinhLuanModel model)
    {
        throw new NotImplementedException();
    }
}