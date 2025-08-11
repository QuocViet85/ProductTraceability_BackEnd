namespace App.Database;

public interface IBaseRepository<TModel>
{
    public Task<List<TModel>> GetManyAsync(int pageNumber, int limit, string search, bool descending);

    public Task<int> GetTotalAsync();

    public Task<int> GetMyTotalAsync(Guid userId);

    public Task<TModel> GetOneByIdAsync(Guid id);

    public Task<List<TModel>> GetMyManyAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task<int> CreateAsync(TModel model);

    public Task<int> UpdateAsync(TModel model);

    public Task<int> DeleteAsync(TModel model);

    public Task<bool> CheckExistByIdAsync(Guid id);
}