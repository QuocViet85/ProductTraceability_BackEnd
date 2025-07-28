namespace App.Database;

public interface IBaseRepository<TModel>
{
    public Task<List<TModel>> GetManyAsync(int pageNumber, int limit, string search);

    public Task<int> GetTotalAsync();

    public Task<int> GetMyTotalAsync(string userId);

    public Task<TModel> GetOneAsync(Guid id);

    public Task<List<TModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search);

    public Task<int> CreateAsync(TModel model);

    public Task<int> UpdateAsync(TModel model);

    public Task<int> DeleteAsync(TModel model);
}