using App.Areas.DanhMuc.Models;
using App.Areas.DoanhNghiep.Models;
using App.Areas.SanPham.Models;
using App.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;


namespace App.Areas.SanPham.Repositories;

public class SanPhamRepository : ISanPhamRepository
{
    private readonly AppDBContext _dbContext;

    public SanPhamRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<SanPhamModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams;

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(p => p.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoAsync()
    {
        return await _dbContext.SanPhams.CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangDanhMucAsync(Guid dm_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_DM_Id == dm_id);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangDanhMucAsync(Guid dm_id)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_DM_Id == dm_id).CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams;

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        var predicate = PredicateBuilder.New<SanPhamModel>();

        List<ChuDoanhNghiepModel> listDoanhNghiepCuaNguoiDung = await _dbContext.ChuDoanhNghieps.Where(cdn => cdn.CDN_ChuDN_Id == userId).ToListAsync();

        foreach (var chuDoanhNghiep in listDoanhNghiepCuaNguoiDung)
        {
            predicate.Or(sp => sp.SP_DN_SoHuu_Id == chuDoanhNghiep.CDN_DN_Id);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Where(predicate).Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoCuaNguoiDungAsync(Guid userId)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams;

        var predicate = PredicateBuilder.New<SanPhamModel>();

        List<ChuDoanhNghiepModel> listDoanhNghiepCuaNguoiDung = await _dbContext.ChuDoanhNghieps.Where(cdn => cdn.CDN_ChuDN_Id == userId).ToListAsync();

        foreach (var chuDoanhNghiep in listDoanhNghiepCuaNguoiDung)
        {
            predicate.Or(sp => sp.SP_DN_SoHuu_Id == chuDoanhNghiep.CDN_DN_Id);
        }

        return await querySanPhams.Where(predicate).CountAsync();
    }

    public async Task<SanPhamModel> LayMotBangIdAsync(Guid id)
    {
        IQueryable<SanPhamModel> querySanPham = _dbContext.SanPhams.Where(sp => sp.SP_Id == id);
        querySanPham = IncludeOfSanPham(querySanPham);
        return await querySanPham.FirstOrDefaultAsync();
    }

    public async Task<SanPhamModel> LayMotBangMaTruyXuatAsync(string sp_MaTruyXuat)
    {
        IQueryable<SanPhamModel> querySanPham = _dbContext.SanPhams.Where(sp => sp.SP_MaTruyXuat == sp_MaTruyXuat);
        querySanPham = IncludeOfSanPham(querySanPham);
        return await querySanPham.FirstOrDefaultAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.SanPhams.AnyAsync(sp => sp.SP_Id == id);
    }

    public async Task<bool> KiemTraTonTaiBangMaTruyXuatAsync(string sp_MaTruyXuat, Guid? id)
    {
        if (id == null)
        {
            return await _dbContext.SanPhams.AnyAsync(sp => sp.SP_MaTruyXuat == sp_MaTruyXuat);
        }
        else
        {
            return await _dbContext.SanPhams.AnyAsync(sp => sp.SP_Id != id && sp.SP_MaTruyXuat == sp_MaTruyXuat);
        }
    }

    public async Task<int> ThemAsync(SanPhamModel sanPham)
    {
        _dbContext.SanPhams.Add(sanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> XoaAsync(SanPhamModel sanPham)
    {
        _dbContext.SanPhams.Remove(sanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> SuaAsync(SanPhamModel sanPham)
    {
        _dbContext.SanPhams.Update(sanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepSoHuuAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_DN_SoHuu_Id == dn_id);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangDoanhNghiepSoHuuAsync(Guid dn_id)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_DN_SoHuu_Id == dn_id).CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepVanTaiAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_DN_VanTai_Id == dn_id);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangDoanhNghiepVanTaiAsync(Guid dn_id)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_DN_VanTai_Id == dn_id).CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepSanXuatAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_DN_SanXuat_Id == dn_id);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangDoanhNghiepSanXuatAsync(Guid dn_id)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_DN_SanXuat_Id == dn_id).CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangNguoiPhuTrachAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_NguoiPhuTrach_Id == userId);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangNguoiPhuTrachAsync(Guid userId)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_NguoiPhuTrach_Id == userId).CountAsync();
    }

    public async Task<List<SanPhamModel>> LayNhieuBangNhaMayAsync(Guid nm_id, int pageNumber, int limit, string search, bool descending)
    {
        IQueryable<SanPhamModel> querySanPhams = _dbContext.SanPhams.Where(sp => sp.SP_NM_Id == nm_id);

        if (descending)
        {
            querySanPhams = querySanPhams.OrderByDescending(sp => sp.SP_NgayTao);
        }
        else
        {
            querySanPhams = querySanPhams.OrderBy(sp => sp.SP_NgayTao);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            querySanPhams = querySanPhams.Where(sp => sp.SP_Ten.Contains(search));
        }

        querySanPhams = querySanPhams.Skip((pageNumber - 1) * limit).Take(limit);

        List<SanPhamModel> listSanPhams = await querySanPhams.ToListAsync();

        return listSanPhams;
    }

    public async Task<int> LayTongSoBangNhaMayAsync(Guid nm_id)
    {
        return await _dbContext.SanPhams.Where(sp => sp.SP_NM_Id == nm_id).CountAsync();
    }

    private IQueryable<SanPhamModel> IncludeOfSanPham(IQueryable<SanPhamModel> querySanPham)
    {
        return querySanPham.Include(sp => sp.SP_DM)
                            .Include(sp => sp.SP_DN_SoHuu)
                            .Include(sp => sp.SP_DN_VanTai)
                            .Include(sp => sp.SP_DN_SanXuat)
                            .Include(sp => sp.SP_NM);
    }

    public async Task<bool> KiemTraTonTaiBangMaVachAsync(string sp_MaVach, Guid? id)
    {
        if (sp_MaVach == null)
        {
            return false;
        }

        if (id == null)
        {
            return await _dbContext.SanPhams.AnyAsync(sp => sp.SP_MaVach == sp_MaVach);
        }
        else
        {
            return await _dbContext.SanPhams.AnyAsync(sp => sp.SP_Id != id && sp.SP_MaVach == sp_MaVach);
        }
    }

    public async Task<int> ThemSaoAsync(SaoSanPhamModel saoSanPham)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM tblSaoSanPham WHERE SSP_SP_Id = {0}", saoSanPham.SSP_SP_Id);
        await _dbContext.SaoSanPhams.AddAsync(saoSanPham);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<double> LaySoSaoAsync(Guid id)
    {
        return await _dbContext.Database.SqlQueryRaw<double>("SELECT dbo.TinhSaoSanPham({0}) AS Value", id).FirstOrDefaultAsync();
    }
}