using System.Security.Claims;
using App.Areas.BaiViet.Model;
using App.Services;

namespace App.Areas.BaiViet.Services;

public interface IBaiVietService : IBaseService<BaiVietModel>
{
    public Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<BaiVietModel> listItems)> LayNhieuCuaNguoiDungAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task SuaAsync(Guid id, string noiDung, ClaimsPrincipal userNowFromJwt);
}