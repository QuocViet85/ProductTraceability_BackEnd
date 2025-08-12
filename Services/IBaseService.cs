using System.Security.Claims;

namespace App.Services;

public interface IBaseService<TModel>
{
    public Task<(int totalItems, List<TModel> listItems)> LayNhieuAsync(int pageNumber, int limit, string search, bool descending);
    public Task<TModel> LayMotBangIdAsync(Guid id);
    public Task<(int totalItems, List<TModel> listItems)> LayNhieuCuaToiAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending);
    public Task ThemAsync(TModel TModel, ClaimsPrincipal userNowFromJwt);
    public Task SuaAsync(Guid id, TModel TModel, ClaimsPrincipal userNowFromJwt);
    public Task XoaAsync(Guid id, ClaimsPrincipal userNowFromJwt);
}