using System.Data.Common;
using App.Areas.BinhLuan.Models;
using App.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.BinhLuan.Repositories;

public class BinhLuanRepository : IBinhLuanRepository
{
    private readonly AppDBContext _dbContext;

    public BinhLuanRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BinhLuanModel>> LayNhieuBangSanPhamAsync(Guid sp_id, int soSao, int pageNumber, int limit)
    {
        IQueryable<BinhLuanModel> queryBinhLuans = _dbContext.BinhLuans
                    .Where(bl => bl.BL_SP_Id == sp_id)
                    .OrderByDescending(bl => bl.BL_NgayTao)
                    .Include(bl => bl.BL_NguoiTao);

        if (soSao >= 1)
        {
            var predicate = PredicateBuilder.New<BinhLuanModel>();

            var listSaoSanPhams = await _dbContext.SaoSanPhams.Where(ssp => ssp.SSP_SP_Id == sp_id && ssp.SSP_SoSao == soSao).ToListAsync();

            foreach (var saoSanPham in listSaoSanPhams)
            {
                predicate.Or(bl => bl.BL_NguoiTao_Id == saoSanPham.SSP_NguoiTao_Id);
            }

            queryBinhLuans = queryBinhLuans.Where(predicate);
        }

        return await queryBinhLuans
                    .Skip((pageNumber - 1) * limit)
                    .Take(limit)
                    .ToListAsync();
    }

    public async Task<int> LayTongSoBangSanPhamAsync(Guid sp_id, int soSao)
    {
        IQueryable<BinhLuanModel> queryBinhLuans = _dbContext.BinhLuans
                    .Where(bl => bl.BL_SP_Id == sp_id);

        if (soSao >= 1)
        {
            var predicate = PredicateBuilder.New<BinhLuanModel>();

            var listSaoSanPhams = await _dbContext.SaoSanPhams.Where(ssp => ssp.SSP_SP_Id == sp_id && ssp.SSP_SoSao == soSao).ToListAsync();

            foreach (var saoSanPham in listSaoSanPhams)
            {
                predicate.Or(bl => bl.BL_NguoiTao_Id == saoSanPham.SSP_NguoiTao_Id);
            }

            queryBinhLuans = queryBinhLuans.Where(predicate);
        }
        
        return await queryBinhLuans.CountAsync();
    }

    public async Task<List<BinhLuanModel>> LayNhieuBangNguoiDungAsync(Guid userId, int pageNumber, int limit)
    {
        return await _dbContext.BinhLuans
                    .Where(bl => bl.BL_NguoiTao_Id == userId)
                    .OrderByDescending(bl => bl.BL_NgayTao)
                    .Include(bl => bl.BL_SP)
                    .Skip((pageNumber - 1) * limit)
                    .Take(limit)
                    .ToListAsync();
    }

    public async Task<int> LayTongSoBangNguoiDungAsync(Guid userId)
    {
        return await _dbContext.BinhLuans.Where(bl => bl.BL_NguoiTao_Id == userId).CountAsync();
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

    public async Task<BinhLuanModel> LayMotBangIdAsync(Guid id)
    {
        return await _dbContext.BinhLuans.Where(bl => bl.BL_Id == id).Include(bl => bl.BL_NguoiTao).FirstOrDefaultAsync();
    }

    public async Task<bool> KiemTraTonTaiBangIdAsync(Guid id)
    {
        return await _dbContext.BinhLuans.AnyAsync(c => c.BL_Id == id);
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

    public Task<List<BinhLuanModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> SuaAsync(BinhLuanModel model)
    {
        throw new NotImplementedException();
    }
}