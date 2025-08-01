using App.Areas.Categories.DTO;
using App.Services;

namespace App.Areas.Categories.Services;

public interface ICategoryService : IBaseService<CategoryDTO>
{
    public Task<CategoryDTO> GetOneByNameAsync(string name);
}