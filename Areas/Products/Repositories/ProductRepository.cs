using App.Areas.Enterprises.Models;
using App.Areas.Products.Models;
using App.Database;
using LinqKit;
using Microsoft.EntityFrameworkCore;


namespace App.Areas.Products.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDBContext _dbContext;

    public ProductRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<ProductModel>> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.Products.CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByCategoryAsync(Guid categoryId)
    {
        return await _dbContext.Products.Where(p => p.CategoryId == categoryId).CountAsync();
    }

    public async Task<List<ProductModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products;

        var predicate = PredicateBuilder.New<ProductModel>();

        predicate.Or(p => p.OwnerIndividualEnterpriseId == userId);

        List<EnterpriseUserModel> listMyEnterprises = await _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).ToListAsync();

        foreach (var myEnterprise in listMyEnterprises)
        {
            predicate.Or(p => p.OwnerEnterpriseId == myEnterprise.EnterpriseId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Where(predicate).Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetMyTotalAsync(string userId)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products;

        var predicate = PredicateBuilder.New<ProductModel>();

        predicate.Or(p => p.OwnerIndividualEnterpriseId == userId);

        List<EnterpriseUserModel> listMyEnterprises = await _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userId).ToListAsync();

        foreach (var myEnterprise in listMyEnterprises)
        {
            predicate.Or(p => p.OwnerEnterpriseId == myEnterprise.EnterpriseId);
        }

        return await queryProducts.Where(predicate).CountAsync();
    }

    public async Task<ProductModel> GetOneByIdAsync(Guid id)
    {
        IQueryable<ProductModel> queryProduct = _dbContext.Products.Where(p => p.Id == id);
        queryProduct = IncludeOfProduct(queryProduct);
        return await queryProduct.FirstOrDefaultAsync();
    }

    public async Task<ProductModel> GetOneByTraceCodeAsync(string traceCode)
    {
        IQueryable<ProductModel> queryProduct = _dbContext.Products.Where(p => p.TraceCode == traceCode);
        queryProduct = IncludeOfProduct(queryProduct);
        return await queryProduct.FirstOrDefaultAsync();
    }

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> CheckExistByTraceCode(string traceCode)
    {
        return await _dbContext.Products.AnyAsync(p => p.TraceCode == traceCode);
    }

    public async Task<bool> CheckExistExceptThisByTraceCode(Guid id, string traceCode)
    {
        return await _dbContext.Products.AnyAsync(p => p.TraceCode == traceCode && p.Id != id);
    }

    public async Task<int> CreateAsync(ProductModel product)
    {
        _dbContext.Products.Add(product);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(ProductModel product)
    {
        _dbContext.Products.Remove(product);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(ProductModel product)
    {
        _dbContext.Products.Update(product);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ProductModel>> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.OwnerIndividualEnterpriseId == individualEnterpriseId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByOwnerIndividualEnterpriseAsync(string individualEnterpriseId)
    {
        return await _dbContext.Products.Where(p => p.OwnerIndividualEnterpriseId == individualEnterpriseId).CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.OwnerEnterpriseId == enterpriseId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByOwnerEnterpriseAsync(Guid enterpriseId)
    {
        return await _dbContext.Products.Where(p => p.OwnerEnterpriseId == enterpriseId).CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.CarrierEnterpriseId == enterpriseId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByCarrierEnterpriseAsync(Guid enterpriseId)
    {
        return await _dbContext.Products.Where(p => p.CarrierEnterpriseId == enterpriseId).CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.ProducerEnterpriseId == enterpriseId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByProducerEnterpriseAsync(Guid enterpriseId)
    {
        return await _dbContext.Products.Where(p => p.ProducerEnterpriseId== enterpriseId).CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.ResponsibleUserId == userId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByResponsibleUserAsync(string userId)
    {
        return await _dbContext.Products.Where(p => p.ResponsibleUserId == userId).CountAsync();
    }

    public async Task<List<ProductModel>> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search)
    {
        IQueryable<ProductModel> queryProducts = _dbContext.Products.Where(p => p.FactoryId == factoryId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryProducts = queryProducts.Where(p => p.Name.Contains(search));
        }

        queryProducts = queryProducts.Skip((pageNumber - 1) * limit).Take(limit);

        List<ProductModel> listProducts = await queryProducts.ToListAsync();

        return listProducts;
    }

    public async Task<int> GetTotalByFactoryAsync(Guid factoryId)
    {
        return await _dbContext.Products.Where(p => p.FactoryId == factoryId).CountAsync();
    }

    private IQueryable<ProductModel> IncludeOfProduct(IQueryable<ProductModel> queryProduct)
    {
        return queryProduct.Include(p => p.Category)
                            .Include(p => p.OwnerIndividualEnterprise)
                            .Include(p => p.OwnerEnterprise)
                            .Include(p => p.ProducerEnterprise)
                            .Include(p => p.CarrierEnterprise)
                            .Include(p => p.ResponsibleUser)
                            .Include(p => p.CreatedUser)
                            .Include(p => p.Factory)
                            .Include(p => p.UpdatedUser);
    }
}