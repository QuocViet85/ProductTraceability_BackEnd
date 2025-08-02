using App.Areas.IndividualEnterprises.Model;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.IndividualEnterprises.Repositories;

public class IndividualEnterpiseRepository : IIndividualEnterpiseRepository
{
    private readonly AppDBContext _dbContext;

    public IndividualEnterpiseRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<IndividualEnterpriseModel>> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<IndividualEnterpriseModel> queryIndividualEnterprises = _dbContext.IndividualEnterprises;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryIndividualEnterprises = queryIndividualEnterprises.Where(ie => ie.Name.Contains(search) || ie.Type.Contains(search) || ie.PhoneNumber.Contains(search));
        }

        queryIndividualEnterprises = queryIndividualEnterprises.Skip((pageNumber - 1) * limit).Take(limit);

        List<IndividualEnterpriseModel> individualEnterprises = await _dbContext.IndividualEnterprises.ToListAsync();

        return individualEnterprises;
    }
    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.IndividualEnterprises.CountAsync();
    }

    public async Task<IndividualEnterpriseModel> GetOneByIdAsync(string id)
    {
        return await _dbContext.IndividualEnterprises.Where(ie => ie.OwnerUserId == id).Include(ie => ie.OwnerUser).Include(ie => ie.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<IndividualEnterpriseModel> GetMyOneAsync(string userId)
    {
        return await _dbContext.IndividualEnterprises.Where(ie => ie.OwnerUserId == userId).Include(ie => ie.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(IndividualEnterpriseModel individualEnterprise)
    {
        await _dbContext.IndividualEnterprises.AddAsync(individualEnterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(IndividualEnterpriseModel individualEnterprise)
    {
        _dbContext.IndividualEnterprises.Remove(individualEnterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(IndividualEnterpriseModel individualEnterprise)
    {
        _dbContext.IndividualEnterprises.Update(individualEnterprise);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<IndividualEnterpriseModel> GetOneByIndividualEnterpriseCodeAsync(string individualEnterpiseCode)
    {
        return await _dbContext.IndividualEnterprises.Where(ie => ie.IndividualEnterpriseCode == individualEnterpiseCode).Include(ie => ie.OwnerUser).Include(ie => ie.UpdatedUser).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckExistByIndividualEnterpriseCodeAsync(string individualEnterpiseCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => ie.IndividualEnterpriseCode == individualEnterpiseCode);
    }

    public async Task<bool> CheckExistExceptThisByIndividualEnterpriseCodeAsync(string id, string individualEnterpiseCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => ie.IndividualEnterpriseCode == individualEnterpiseCode && ie.OwnerUserId != id);
    }

    public async Task<bool> CheckUserHadIndividualEnterpiseBeforeAsync(string userId)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => ie.OwnerUserId == userId);
    }

    public async Task<bool> CheckExistByTaxCodeAndGLNCodeAsync(string taxCode, string gLNCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => (taxCode != null && ie.TaxCode == taxCode) || (gLNCode != null && ie.GLNCode == gLNCode));
    }

    public async Task<bool> CheckExistExceptThisByTaxCodeAndGLNCodeAsync(string id, string taxCode, string gLNCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => (taxCode != null && ie.TaxCode == taxCode && ie.OwnerUserId != id) || (gLNCode != null && ie.GLNCode == gLNCode && ie.OwnerUserId != id));
    }

    public Task<List<IndividualEnterpriseModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }
    public Task<int> GetMyTotalAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<IndividualEnterpriseModel> GetOneByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

