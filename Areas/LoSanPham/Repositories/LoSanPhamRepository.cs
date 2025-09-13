using App.Areas.LoSanPham.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.LoSanPham.Repositories;

public class LoSanPhamRepository : ILoSanPhamRepository
{
    private readonly AppDBContext _dbContext;

    public LoSanPhamRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LoSanPhamModel>> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<LoSanPhamModel> queryLoSanPhams = _dbContext.LoSanPhams.Where(lsp => lsp.LSP_SP_Id == sp_Id).Include(lsp => lsp.LSP_NM);

        if (descending)
        {
            queryLoSanPhams = queryLoSanPhams.OrderByDescending(lsp => lsp.LSP_NgaySanXuat);
        }
        else
        {
            queryLoSanPhams = queryLoSanPhams.OrderBy(lsp => lsp.LSP_NgaySanXuat);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryLoSanPhams = queryLoSanPhams.Where(lsp => lsp.LSP_Ten.Contains(search) || lsp.LSP_MaLSP.Contains(search));
        }

        List<LoSanPhamModel> listLoSanPhams = await queryLoSanPhams.Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();

        return listLoSanPhams;
    }

    public async Task<int> LayTongSoBangSanPhamAsync(Guid sp_Id)
    {
        return await _dbContext.LoSanPhams.Where(lsp => lsp.LSP_SP_Id == sp_Id).CountAsync();
    }

    public async Task<LoSanPhamModel> LayMotBangIdAsync(Guid id)
    {
        IQueryable<LoSanPhamModel> queryLoSanPham = _dbContext.LoSanPhams.Where(lsp => lsp.LSP_Id == id);
        queryLoSanPham = IncludeOfLoSanPham(queryLoSanPham);
        return await queryLoSanPham.FirstOrDefaultAsync();
    }

    public async Task<LoSanPhamModel> LayMotBangMaLoSanPhamAsync(string lsp_MaLSP)
    {
        IQueryable<LoSanPhamModel> queryLoSanPham = _dbContext.LoSanPhams.Where(lsp => lsp.LSP_MaLSP == lsp_MaLSP);
        queryLoSanPham = IncludeOfLoSanPham(queryLoSanPham);
        return await queryLoSanPham.FirstOrDefaultAsync();
    }

    public async Task<int> ThemAsync(LoSanPhamModel loSanPham)
    {
        await _dbContext.LoSanPhams.AddAsync(loSanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(LoSanPhamModel loSanPham)
    {
        _dbContext.LoSanPhams.Remove(loSanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(LoSanPhamModel loSanPham)
    {
        _dbContext.LoSanPhams.Update(loSanPham);
        return await _dbContext.SaveChangesAsync();
    }

    private IQueryable<LoSanPhamModel> IncludeOfLoSanPham(IQueryable<LoSanPhamModel> queryLoSanPham)
    {
        return queryLoSanPham.Include(lsp => lsp.LSP_SP)
                            .ThenInclude(sp => sp.SP_DN_SoHuu)
                            .Include(lsp => lsp.LSP_NM);
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.LoSanPhams.AnyAsync(lsp => lsp.LSP_Id == id);
    }

    public async Task<bool> KiemTraTonTaiBangMaLoSanPhamAsync(string lsp_MaLSP, Guid? id = null)
    {
        if (id == null)
        {
            return await _dbContext.LoSanPhams.AnyAsync(lsp => lsp.LSP_MaLSP == lsp_MaLSP);
        }
        else
        {
            return await _dbContext.LoSanPhams.AnyAsync(lsp => lsp.LSP_Id != id && lsp.LSP_MaLSP == lsp_MaLSP);
        }
    }

    //Not Implement
    public Task<List<LoSanPhamModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
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

    public Task<List<LoSanPhamModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}