using App.Areas.Categories.Models;
using App.Database;

namespace App.Areas.Categories.Repositories;

public interface ICategoryRepository : IBaseRepository<CategoryModel>
{
    public Task<bool> CheckExistAsync(string name);

    public Task<bool> CheckExistExceptThisAsync(Guid id, string name);
}