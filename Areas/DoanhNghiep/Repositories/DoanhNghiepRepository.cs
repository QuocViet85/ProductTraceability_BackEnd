using App.Areas.Auth.AuthorizationData;
using App.Areas.DoanhNghiep.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.DoanhNghiep.Repositories;

public class DoanhNghiepRepository : IDoanhNghiepRepository
{
    private readonly AppDBContext _dbContext;
    public DoanhNghiepRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DoanhNghiepModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<DoanhNghiepModel> queryDoanhNghieps = _dbContext.DoanhNghieps;

        if (descending)
        {
            queryDoanhNghieps = queryDoanhNghieps.OrderByDescending(dn => dn.DN_NgayTao);
        }
        else
        {
            queryDoanhNghieps = queryDoanhNghieps.OrderBy(dn => dn.DN_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryDoanhNghieps = queryDoanhNghieps.Where(dn => dn.DN_Ten.Contains(search) || dn.DN_SoDienThoai.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        queryDoanhNghieps = queryDoanhNghieps.Skip((pageNumber - 1) * limit).Take(limit);

        List<DoanhNghiepModel> listDoanhNghieps = await queryDoanhNghieps.ToListAsync();

        return listDoanhNghieps;
    }

    public async Task<List<DoanhNghiepModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<ChuDoanhNghiepModel> queryChuDoanhNghiep = _dbContext.ChuDoanhNghieps.Where(eu => eu.CDN_ChuDN_Id == userId);
        IQueryable<DoanhNghiepModel> queryDoanhNghieps = queryChuDoanhNghiep.Select(eu => eu.CDN_DN);

        if (descending)
        {
            queryDoanhNghieps = queryDoanhNghieps.OrderByDescending(dn => dn.DN_NgayTao);
        }
        else
        {
            queryDoanhNghieps = queryDoanhNghieps.OrderBy(dn => dn.DN_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryDoanhNghieps = queryDoanhNghieps.Where(dn => dn.DN_Ten.Contains(search) || dn.DN_SoDienThoai.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        queryDoanhNghieps = queryDoanhNghieps.Skip((pageNumber - 1) * limit).Take(limit);

        List<DoanhNghiepModel> listDoanhNghieps = await queryDoanhNghieps.ToListAsync();

        return listDoanhNghieps;
    }

    public async Task<DoanhNghiepModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.DoanhNghieps.Where(dn => dn.DN_Id == id).Include(dn => dn.DN_List_CDN).FirstOrDefaultAsync();
    }

    public async Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string dn_MaSoThue)
    {
        return await _dbContext.DoanhNghieps.Where(dn => dn.DN_MaSoThue == dn_MaSoThue).Include(e => e.DN_List_CDN).FirstOrDefaultAsync();
    }

    public async Task<int> LayTongSoAsync()
    {
        return await _dbContext.DoanhNghieps.CountAsync();
    }

    public async Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        return await _dbContext.ChuDoanhNghieps.Where(cdn => cdn.CDN_ChuDN_Id == userId).CountAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.DoanhNghieps.AnyAsync(dn => dn.DN_Id == id);
    }

    public async Task<bool> KiemTraTonTaiBangMaSoThueAsync(string dn_MaSoThue, Guid? id)
    {
        if (id == null)
        {
            return await _dbContext.DoanhNghieps.AnyAsync(dn => dn.DN_MaSoThue == dn_MaSoThue);
        }
        else
        {
            return await _dbContext.DoanhNghieps.AnyAsync(dn => dn.DN_Id != id && dn.DN_MaSoThue == dn_MaSoThue);
        }
    }

    public async Task<bool> KiemTraTonTaiBangMaGLNAsync(string dn_MaGLN, Guid? id)
    {
        if (dn_MaGLN == null)
        {
            return false;
        }

        if (id == null)
        {
            return await _dbContext.DoanhNghieps.AnyAsync(dn => dn.DN_MaGLN == dn_MaGLN);
        }
        else
        {
            return await _dbContext.DoanhNghieps.AnyAsync(dn => dn.DN_Id != id && dn.DN_MaGLN == dn_MaGLN);
        }
    }

    public async Task<bool> KiemTraLaChuDoanhNghiepAsync(Guid id, Guid userId)
    {
        return await _dbContext.ChuDoanhNghieps.AnyAsync(cdn => cdn.CDN_ChuDN_Id == userId && cdn.CDN_DN_Id == id);
    }

    public async Task<int> ThemAsync(DoanhNghiepModel doanhNghiep)
    {
        await _dbContext.DoanhNghieps.AddAsync(doanhNghiep);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(DoanhNghiepModel doanhNghiep)
    {
        _dbContext.DoanhNghieps.Remove(doanhNghiep);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(DoanhNghiepModel doanhNghiep)
    {
        _dbContext.DoanhNghieps.Update(doanhNghiep);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> ThemSoHuuDoanhNghiepAsync(ChuDoanhNghiepModel ChuDoanhNghiep)
    {
        await _dbContext.ChuDoanhNghieps.AddAsync(ChuDoanhNghiep);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM tblChuDoanhNghiep WHERE CDN_ChuDN_Id = {0} AND CDN_DN_Id = {1}", userId, id);
    }

    public async Task<int> TuBoSoHuuDoanhNghiepAsync(Guid id, Guid userId)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM tblChuDoanhNghiep WHERE CDN_ChuDN_Id = {0} AND CDN_DN_Id = {1}", userId, id);
    }

    public async Task<int> XoaPhanQuyenDoanhNghiepAsync(Guid id, Guid userId)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM AspNetUserClaims WHERE UserId = {0} AND ClaimValue LIKE {1}", userId, $"dn%{id}");
    }
}