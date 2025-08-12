namespace App.Database;

public interface IBaseRepository<TModel>
{
    public Task<List<TModel>> LayNhieuAsync(int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoAsync();

    public Task<int> LayTongSoCuaNguoiDungAsync(Guid userId);

    public Task<TModel> LayMotBangIdAsync(Guid id);

    public Task<List<TModel>> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task<int> ThemAsync(TModel model);

    public Task<int> SuaAsync(TModel model);

    public Task<int> XoaAsync(TModel model);

    public Task<bool> KiemTraTonTaiBangIdAsync(Guid id);
}