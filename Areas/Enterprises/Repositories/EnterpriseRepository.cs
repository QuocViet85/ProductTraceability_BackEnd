using App.Areas.Enterprises.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Enterprises.Repositories;

public class EnterpriseRepository : IEnterpriseRepository
{
    private readonly AppDBContext _dbContext;
    public EnterpriseRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<EnterpriseModel>> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<EnterpriseModel> queryEnterprises = _dbContext.Enterprises;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryEnterprises = queryEnterprises.Where(e => e.Name.Contains(search) || e.Type.Contains(search) || e.PhoneNumber.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);

        List<EnterpriseModel> listEnterprises = await queryEnterprises.ToListAsync();

        return listEnterprises;
    }

    public async Task<List<EnterpriseModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        IQueryable<EnterpriseUserModel> queryEnterpriseUser = _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).Include(eu => eu.User);
        List<EnterpriseUserModel> listEnterpriseUser = await queryEnterpriseUser.ToListAsync();
        IQueryable<EnterpriseModel> queryEnterprises = queryEnterpriseUser.Select(eu => eu.Enterprise);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryEnterprises = queryEnterprises.Where(e => e.Name.Contains(search) || e.Type.Contains(search) || e.PhoneNumber.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);

        List<EnterpriseModel> listEnterprises = await queryEnterprises.ToListAsync();

        foreach (var enterprise in listEnterprises)
        {
            enterprise.EnterpriseUsers = listEnterpriseUser;
        }

        return listEnterprises;
    }

    public async Task<EnterpriseModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.Enterprises.Where(e => e.Id == id).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).Include(e => e.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<EnterpriseModel> GetOneByTaxCodeAsync(string taxCode)
    {
        return await _dbContext.Enterprises.Where(e => e.TaxCode == taxCode).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).Include(e => e.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.Enterprises.CountAsync();
    }

    public async Task<int> GetMyTotalAsync(string userId)
    {
        return await _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).CountAsync();
    }

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.Enterprises.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> CheckExistByCodeAsync(string taxCode, string gLNCode)
    {
        return await _dbContext.Enterprises.AnyAsync(e => e.TaxCode == taxCode || (gLNCode != null && e.GLNCode == gLNCode));
    }

    public async Task<bool> CheckExistExceptThisByCodeAsync(Guid id, string taxCode, string gLNCode)
    {
        return await _dbContext.Enterprises.AnyAsync(e => (e.TaxCode == taxCode && e.Id != id) || (gLNCode != null && e.GLNCode == gLNCode && e.Id != id));
    }

    public async Task<bool> CheckIsOwner(Guid id, string userId)
    {
        return await _dbContext.EnterpriseUsers.AnyAsync(eu => eu.UserId == userId && eu.EnterpriseId == id);
    }

    public async Task<int> CreateAsync(EnterpriseModel enterprise)
    {
        await _dbContext.Enterprises.AddAsync(enterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(EnterpriseModel enterprise)
    {
        _dbContext.Enterprises.Remove(enterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(EnterpriseModel enterprise)
    {
        _dbContext.Enterprises.Update(enterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddOwnershipAsync(EnterpriseUserModel enterpriseUser)
    {
        await _dbContext.EnterpriseUsers.AddAsync(enterpriseUser);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteOwnershipAsync(Guid id, string userId)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM EnterpriseUser WHERE UserId = {0} AND EnterpriseId = {1}", userId, id);
    }

    public async Task<int> GiveUpOwnershipAsync(Guid id, string userId)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM EnterpriseUser WHERE UserId = {0} AND EnterpriseId = {1}", userId, id);
    }
}