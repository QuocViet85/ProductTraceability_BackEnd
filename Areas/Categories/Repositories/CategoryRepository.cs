using App.Areas.Categories.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Categories.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDBContext _dbContext;

    public CategoryRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<CategoryModel>> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<CategoryModel> queryCategories = _dbContext.Categories;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryCategories = queryCategories.Where(c => c.Name.Contains(search));
        }

        queryCategories = queryCategories.Skip((pageNumber - 1) * limit).Take(limit);

        List<CategoryModel> listCategories = await queryCategories.Include(c => c.User).ToListAsync();

        return listCategories;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.Categories.CountAsync();
    }

    public async Task<List<CategoryModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search)
    {
        IQueryable<CategoryModel> queryCategories = _dbContext.Categories.Where(c => c.UserId == userId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryCategories = queryCategories.Where(c => c.Name.Contains(search));
        }

        queryCategories.Skip((pageNumber - 1) * limit).Take(limit);

        List<CategoryModel> listCategories = await queryCategories.ToListAsync();

        return listCategories;
    }
    
    public async Task<int> GetMyTotalAsync(string userId)
    {
        return await _dbContext.Categories.Where(c => c.UserId == userId).CountAsync();
    }

    public async Task<CategoryModel> GetOneAsync(Guid id)
    {
        return await _dbContext.Categories.Where(c => c.Id == id).Include(c => c.User).FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CategoryModel category)
    {
        await _dbContext.Categories.AddAsync(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(CategoryModel category)
    {
        _dbContext.Categories.Remove(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(CategoryModel category)
    {
        _dbContext.Categories.Update(category);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> CheckExistAsync(string name)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> CheckExistExceptThisAsync(Guid id, string name)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != id);
    }
}