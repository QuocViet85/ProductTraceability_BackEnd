using App.Areas.SuKienTruyXuat.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.SuKienTruyXuat.Repositories;

public class SuKienTruyXuatRepository : ISuKienTruyXuatRepository
{
    private readonly AppDBContext _dbContext;

    public SuKienTruyXuatRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SuKienTruyXuatModel>> LayNhieuBangLoSanPhamAsync(Guid lsp_Id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SuKienTruyXuatModel> querySuKienTruyXuats = _dbContext.SuKienTruyXuats.Where(sk => sk.SK_LSP_Id == lsp_Id);

        if (descending)
        {
            querySuKienTruyXuats = querySuKienTruyXuats.OrderByDescending(sk => sk.SK_ThoiGian);
        }
        else
        {
            querySuKienTruyXuats = querySuKienTruyXuats.OrderBy(sk => sk.SK_ThoiGian);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySuKienTruyXuats = querySuKienTruyXuats.Where(sk => sk.SK_Ten.Contains(search) || sk.SK_MaSK.Contains(search) || sk.SK_MoTa.Contains(search));
        }

        return await querySuKienTruyXuats.Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();
    }

    public async Task<int> LayTongSoBangLoSanPhamAsync(Guid lsp_Id)
    {
        return await _dbContext.SuKienTruyXuats.Where(sk => sk.SK_LSP_Id == lsp_Id).CountAsync();
    }

    public async Task<SuKienTruyXuatModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.SuKienTruyXuats.Where(sk => sk.SK_Id == id).Include(sk => sk.SK_LSP).ThenInclude(lsp => lsp.LSP_SP).FirstOrDefaultAsync();
    }

    public async Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK)
    {
        return await _dbContext.SuKienTruyXuats.Where(sk => sk.SK_MaSK == sk_MaSK).Include(sk => sk.SK_LSP).ThenInclude(lsp => lsp.LSP_SP).FirstOrDefaultAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.SuKienTruyXuats.AnyAsync(sk => sk.SK_Id == id);
    }

    public async Task<bool> KiemTraTonTaiBangMaSuKienAsync(string sk_MaSK, Guid? id = null)
    {
        if (id == null)
        {
            return await _dbContext.SuKienTruyXuats.AnyAsync(sk => sk.SK_MaSK == sk_MaSK);
        }
        else
        {
            return await _dbContext.SuKienTruyXuats.AnyAsync(sk => sk.SK_Id != id && sk.SK_MaSK == sk_MaSK);
        }
        
    }

    public async Task<int> ThemAsync(SuKienTruyXuatModel suKienTruyXuat)
    {
        await _dbContext.SuKienTruyXuats.AddAsync(suKienTruyXuat);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(SuKienTruyXuatModel suKienTruyXuat)
    {
        _dbContext.SuKienTruyXuats.Update(suKienTruyXuat);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(SuKienTruyXuatModel suKienTruyXuat)
    {
        _dbContext.SuKienTruyXuats.Remove(suKienTruyXuat);
        return await _dbContext.SaveChangesAsync();
    }

    //Not Implement

    public Task<List<SuKienTruyXuatModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
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

    public Task<List<SuKienTruyXuatModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}