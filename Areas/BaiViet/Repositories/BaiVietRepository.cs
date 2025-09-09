using App.Areas.BaiViet.Model;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.BaiViet.Repositories;

public class BaiVietRepository : IBaiVietRepository
{
    private readonly AppDBContext _dbContext;

    public BaiVietRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BaiVietModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<BaiVietModel> queryBaiViets = _dbContext.BaiViets;

        if (descending)
        {
            queryBaiViets = queryBaiViets.OrderByDescending(bv => bv.BV_NgayTao);
        }
        else
        {
            queryBaiViets = queryBaiViets.OrderBy(bv => bv.BV_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryBaiViets = queryBaiViets.Where(bv => bv.BV_NoiDung.Contains(search));
        }

        queryBaiViets.Skip((pageNumber - 1) * limit).Take(limit);

        List<BaiVietModel> listBaiViets = await queryBaiViets.ToListAsync();

        return listBaiViets;
    }
    public async Task<int> LayTongSoAsync()
    {
        return await _dbContext.BaiViets.CountAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.BaiViets.AnyAsync(bv => bv.BV_Id == id);
    }

    public async Task<BaiVietModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.BaiViets.Where(bv => bv.BV_Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<BaiVietModel>> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<BaiVietModel> queryBaiViets = _dbContext.BaiViets.Where(bv => bv.BV_SP_Id == sp_id);

        if (descending)
        {
            queryBaiViets = queryBaiViets.OrderByDescending(bv => bv.BV_NgayTao);
        }
        else
        {
            queryBaiViets = queryBaiViets.OrderBy(bv => bv.BV_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryBaiViets = queryBaiViets.Where(bv => bv.BV_NoiDung.Contains(search));
        }

        queryBaiViets.Skip((pageNumber - 1) * limit).Take(limit);

        List<BaiVietModel> listBaiViets = await queryBaiViets.ToListAsync();

        return listBaiViets;
    }

    public async Task<int> LayTongSoBangSanPhamAsync(Guid sp_id)
    {
        return await _dbContext.BaiViets.Where(bv => bv.BV_SP_Id == sp_id).CountAsync();
    }

    public async Task<List<BaiVietModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<BaiVietModel> queryBaiViets = _dbContext.BaiViets.Where(bv => bv.BV_NguoiTao_Id == userId);

        if (descending)
        {
            queryBaiViets = queryBaiViets.OrderByDescending(bv => bv.BV_NgayTao);
        }
        else
        {
            queryBaiViets = queryBaiViets.OrderBy(bv => bv.BV_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryBaiViets = queryBaiViets.Where(bv => bv.BV_NoiDung.Contains(search));
        }

        queryBaiViets.Skip((pageNumber - 1) * limit).Take(limit);

        List<BaiVietModel> listBaiViets = await queryBaiViets.ToListAsync();

        return listBaiViets;
    }

    public async Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        return await _dbContext.BaiViets.Where(bv => bv.BV_NguoiTao_Id == userId).CountAsync();
    }

    public async Task<int> ThemAsync(BaiVietModel baiViet)
    {
        await _dbContext.BaiViets.AddAsync(baiViet);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(BaiVietModel baiViet)
    {
        _dbContext.BaiViets.Update(baiViet);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(BaiVietModel baiViet)
    {
        _dbContext.BaiViets.Remove(baiViet);
        return await _dbContext.SaveChangesAsync();
    }
}