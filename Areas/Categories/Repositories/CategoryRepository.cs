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

    public async Task<List<CategoryModel>> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        List<CategoryModel> listCategories = await _dbContext.Categories.Where(c => c.IsParent == true).Include(c => c.ChildCategories).ToListAsync();

        return listCategories;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _dbContext.Categories.CountAsync();
    }

    public async Task<CategoryModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.Categories
        .Where(c => c.Id == id)
        .Include(c => c.CreatedUser)
        .Include(c => c.UpdatedUser)
        .Include(c => c.ParentCategory)
        .Include(c => c.ChildCategories)
        .FirstOrDefaultAsync();
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

    public async Task<bool> CheckExistByNameAsync(string name)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> CheckExistExceptThisByNameAsync(Guid id, string name)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != id);
    }

    public async Task<CategoryModel> GetOneByNameAsync(string name)
    {
        return await _dbContext.Categories
                        .Where(c => c.Name == name)
                        .Include(c => c.CreatedUser)
                        .Include(c => c.UpdatedUser)
                        .Include(c => c.ParentCategory)
                        .Include(c => c.ChildCategories)
                        .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<int> GetMyTotalAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CategoryModel>> GetMyManyAsync(Guid userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}