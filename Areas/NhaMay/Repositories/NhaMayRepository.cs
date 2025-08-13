using App.Areas.DoanhNghiep.Models;
using App.Areas.NhaMay.Models;
using App.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.NhaMay.Repositories;

public class NhaMayRepository : INhaMayRepository
{
    private readonly AppDBContext _dbContext;

    public NhaMayRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NhaMayModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<NhaMayModel> queryNhaMays = _dbContext.NhaMays;

        if (descending)
        {
            queryNhaMays = queryNhaMays.OrderByDescending(nm => nm.NM_NgayTao);
        }
        else
        {
            queryNhaMays = queryNhaMays.OrderBy(nm => nm.NM_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryNhaMays = queryNhaMays.Where(nm => nm.NM_Ten.Contains(search));
        }

        queryNhaMays = queryNhaMays.Skip((pageNumber - 1) * limit).Take(limit);
        List<NhaMayModel> listNhaMays = await queryNhaMays.ToListAsync();

        return listNhaMays;
    }

    public async Task<int> LayTongSoAsync()
    {
        return await _dbContext.NhaMays.CountAsync();
    }

    public async Task<List<NhaMayModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<NhaMayModel> queryNhaMays = _dbContext.NhaMays;

        if (descending)
        {
            queryNhaMays = queryNhaMays.OrderByDescending(nm => nm.NM_NgayTao);
        }
        else
        {
            queryNhaMays = queryNhaMays.OrderBy(nm => nm.NM_NgayTao);
        }

        var predicate = PredicateBuilder.New<NhaMayModel>();

        List<ChuDoanhNghiepModel> listDoanhNghiepCuaNguoiDung = await _dbContext.ChuDoanhNghieps.Where(cdn => cdn.CDN_ChuDN_Id == userId).ToListAsync();

        foreach (var doanhNghiep in listDoanhNghiepCuaNguoiDung)
        {
            predicate.Or(nm => nm.NM_DN_Id == doanhNghiep.CDN_DN_Id);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryNhaMays = queryNhaMays.Where(f => f.NM_Ten.Contains(search));
        }

        queryNhaMays = queryNhaMays.Where(predicate).Skip((pageNumber - 1) * limit).Take(limit);
        List<NhaMayModel> listNhaMays = await queryNhaMays.ToListAsync();

        return listNhaMays;
    }

    public async Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        IQueryable<NhaMayModel> queryNhaMays = _dbContext.NhaMays;

        var predicate = PredicateBuilder.New<NhaMayModel>();

        List<ChuDoanhNghiepModel> listDoanhNghiepCuaNguoiDung = await _dbContext.ChuDoanhNghieps.Where(cdn => cdn.CDN_ChuDN_Id == userId).ToListAsync();

        foreach (var doanhNghiep in listDoanhNghiepCuaNguoiDung)
        {
            predicate.Or(nm => nm.NM_DN_Id == doanhNghiep.CDN_DN_Id);
        }

        return await queryNhaMays.CountAsync();
    }
    public async Task<int> ThemAsync(NhaMayModel nhaMay)
    {
        await _dbContext.NhaMays.AddAsync(nhaMay);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(NhaMayModel nhaMay)
    {
        _dbContext.NhaMays.Remove(nhaMay);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<NhaMayModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.NhaMays.Where(nm => nm.NM_Id == id).Include(nm => nm.NM_DN).FirstOrDefaultAsync();
    }

    public async Task<int> SuaAsync(NhaMayModel nhaMay)
    {
        _dbContext.NhaMays.Update(nhaMay);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.NhaMays.AnyAsync(nm => nm.NM_Id == id);
    }

    public async Task<bool> KiemTraTonTaiBangMaNhaMayAsync(string nm_MaNM, Guid? id)
    {
        if (id == null)
        {
            return await _dbContext.NhaMays.AnyAsync(nm => nm.NM_MaNM == nm_MaNM);
        }
        else
        {
            return await _dbContext.NhaMays.AnyAsync(nm => nm.NM_MaNM == nm_MaNM && nm.NM_Id != id);
        }
    }

    public async Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM)
    {
        return await _dbContext.NhaMays.Where(nm => nm.NM_MaNM == nm_MaNM).Include(nm => nm.NM_DN).FirstOrDefaultAsync();
    }
}