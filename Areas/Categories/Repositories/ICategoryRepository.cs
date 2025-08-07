using App.Areas.Categories.Models;
using App.Database;

namespace App.Areas.Categories.Repositories;

public interface ICategoryRepository : IBaseRepository<CategoryModel>
{
    public Task<bool> CheckExistByNameAsync(string name);
    public Task<bool> CheckExistExceptThisByNameAsync(Guid id, string name);
    public Task<CategoryModel> GetOneByNameAsync(string name);
}