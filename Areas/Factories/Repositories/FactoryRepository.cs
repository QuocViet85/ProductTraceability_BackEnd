using App.Areas.Enterprises.Models;
using App.Areas.Factories.Models;
using App.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace App.Areas.Factories.Repositories;

public class FactoryRepository : IFactoryRepository
{
    private readonly AppDBContext _dbContext;

    public FactoryRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FactoryModel>> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<FactoryModel> queryFactories = _dbContext.Factories;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryFactories = queryFactories.Where(f => f.Name.Contains(search));
        }

        queryFactories = queryFactories.Skip((pageNumber - 1) * limit).Take(limit);
        List<FactoryModel> listFactories = await queryFactories.Include(f => f.IndividualEnterprise).Include(f => f.Enterprise).ToListAsync();

        return listFactories;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.Factories.CountAsync();
    }

    public async Task<List<FactoryModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        IQueryable<FactoryModel> queryFactories = _dbContext.Factories;

        var predicate = PredicateBuilder.New<FactoryModel>();

        predicate.Or(f => f.IndividualEnterpriseId == userId);

        List<EnterpriseUserModel> listMyEnterprises = await _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).ToListAsync();

        foreach (var myEnterprise in listMyEnterprises)
        {
            predicate.Or(f => f.EnterpriseId == myEnterprise.EnterpriseId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            predicate.Or(f => f.Name.Contains(search));
        }

        queryFactories = queryFactories.Where(predicate).Skip((pageNumber - 1) * limit).Take(limit).Include(f => f.IndividualEnterprise).Include(f => f.Enterprise);
        List<FactoryModel> listFactories = await queryFactories.ToListAsync();

        return listFactories;
    }

    public async Task<int> GetMyTotalAsync(string userId)
    {
        IQueryable<FactoryModel> queryFactories = _dbContext.Factories;

        var predicate = PredicateBuilder.New<FactoryModel>();

        predicate.Or(f => f.IndividualEnterpriseId == userId);

        List<EnterpriseUserModel> listMyEnterprises = await _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).ToListAsync();

        foreach (var myEnterprise in listMyEnterprises)
        {
            predicate.Or(f => f.EnterpriseId == myEnterprise.EnterpriseId);
        }

        return await queryFactories.CountAsync();
    }
    public async Task<int> CreateAsync(FactoryModel factory)
    {
        await _dbContext.Factories.AddAsync(factory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(FactoryModel factory)
    {
        _dbContext.Factories.Remove(factory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<FactoryModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.Factories.Where(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> UpdateAsync(FactoryModel factory)
    {
        _dbContext.Factories.Update(factory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> CheckExistByFactoryCodeAsync(string factoryCode)
    {
        return await _dbContext.Factories.AnyAsync(f => f.FactoryCode == factoryCode);
    }

    public async Task<FactoryModel> GetOneByFactoryCodeAsync(string factoryCode)
    {
        return await _dbContext.Factories.Where(f => f.FactoryCode == factoryCode).FirstOrDefaultAsync();
    }
}