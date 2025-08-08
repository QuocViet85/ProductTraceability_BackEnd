using System.Security.Claims;

namespace App.Services;

public interface IBaseService<TDto>
{
    public Task<(int totalItems, List<TDto> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending);
    public Task<TDto> GetOneByIdAsync(Guid id);
    public Task<(int totalItems, List<TDto> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending);
    public Task CreateAsync(TDto TDto, ClaimsPrincipal userNowFromJwt);
    public Task UpdateAsync(Guid id, TDto TDto, ClaimsPrincipal userNowFromJwt);
    public Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt);
}