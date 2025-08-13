using App.Areas.DanhMuc.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.DanhMuc.Repositories;

public class DanhMucRepository : IDanhMucRepository
{
    private readonly AppDBContext _dbContext;

    public DanhMucRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DanhMucModel>> LayTatCaAsync()
    {
        return await _dbContext.DanhMucs.AsNoTracking().ToListAsync();
    }

    public async Task<List<DanhMucModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        List<DanhMucModel> tatCaDanhMuc = await LayTatCaAsync();

        List<DanhMucModel> listDanhMucs = tatCaDanhMuc.Where(dm => dm.DM_DMCha_Id == null).ToList();

        foreach (var danhMuc in listDanhMucs)
        {
            SetDanhMucConCuaDanhMucCha(danhMuc, tatCaDanhMuc);
        }

        return listDanhMucs;
    }

    public void SetDanhMucConCuaDanhMucCha(DanhMucModel danhMucCha, List<DanhMucModel> tatCaDanhMuc)
    {
        List<DanhMucModel> listDanhMucCons = tatCaDanhMuc.Where(dm => dm.DM_DMCha_Id == danhMucCha.DM_Id).ToList();
        danhMucCha.DM_List_DMCon = listDanhMucCons;

        foreach (var danhMucCon in listDanhMucCons)
        {
            SetDanhMucConCuaDanhMucCha(danhMucCon, tatCaDanhMuc);
        }
    }

    public async Task<int> LayTongSoAsync()
    {
        return await _dbContext.DanhMucs.CountAsync();
    }

    public async Task<DanhMucModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.DanhMucs
        .Where(dm => dm.DM_Id == id)
        .Include(dm => dm.DM_DMCha)
        .Include(dm => dm.DM_List_DMCon)
        .FirstOrDefaultAsync();
    }

    public async Task<int> ThemAsync(DanhMucModel category)
    {
        await _dbContext.DanhMucs.AddAsync(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(DanhMucModel category)
    {
        _dbContext.DanhMucs.Remove(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(DanhMucModel category)
    {
        _dbContext.DanhMucs.Update(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> KiemTraTonTaiBangTenAsync(string ten, Guid? id = null)
    {
        if (id == null)
        {
            return await _dbContext.DanhMucs.AnyAsync(dm => dm.DM_Ten == ten);
        }
        else
        {
            return await _dbContext.DanhMucs.AnyAsync(dm => dm.DM_Id != id && dm.DM_Ten == ten);
        }  
    }

    public async Task<DanhMucModel> LayMotBangTenAsync(string ten)
    {
        return await _dbContext.DanhMucs
                        .Where(dm => dm.DM_Ten == ten)
                        .Include(dm => dm.DM_DMCha)
                        .Include(dm => dm.DM_List_DMCon)
                        .FirstOrDefaultAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.DanhMucs.AnyAsync(dm => dm.DM_Id == id);
    }

    //

    public Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<DanhMucModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}