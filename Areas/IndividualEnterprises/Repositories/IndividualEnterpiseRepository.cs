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

        List<IndividualEnterpriseModel> individualEnterprises = await _dbContext.IndividualEnterprises.Include(ie => ie.OwnerUser).ToListAsync();

        return individualEnterprises;
    }
    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.IndividualEnterprises.CountAsync();
    }

    public async Task<IndividualEnterpriseModel> GetOneAsync(string id)
    {
        return await _dbContext.IndividualEnterprises.Where(ie => ie.OwnerUserId == id).Include(ie => ie.OwnerUser).FirstOrDefaultAsync();
    }

    public async Task<IndividualEnterpriseModel> GetMyOneAsync(string userId)
    {
        return await _dbContext.IndividualEnterprises.Where(ie => ie.OwnerUserId == userId).Include(ie => ie.OwnerUser).FirstOrDefaultAsync();
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

    public async Task<bool> CheckUserHadIndividualEnterpiseBeforeAsync(string userId)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => ie.OwnerUserId == userId);
    }

    public async Task<bool> CheckExistByCodeAsync(string taxCode, string gLNCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => ie.TaxCode == taxCode || (gLNCode != null && ie.GLNCode == gLNCode));
    }

    public async Task<bool> CheckExistExceptThisByCodeAsync(string id, string taxCode, string gLNCode)
    {
        return await _dbContext.IndividualEnterprises.AnyAsync(ie => (ie.TaxCode == taxCode && ie.OwnerUserId != id) || (gLNCode != null && ie.GLNCode == gLNCode && ie.OwnerUserId != id));
    }

    public Task<List<IndividualEnterpriseModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }
    public Task<int> GetMyTotalAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<IndividualEnterpriseModel> GetOneAsync(Guid id)
    {
        throw new NotImplementedException();
    }

}

