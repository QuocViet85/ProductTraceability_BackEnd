using System.Security.Claims;
using App.Areas.BinhLuan.Models;
using App.Services;

namespace App.Areas.BinhLuan.Services;

public interface IBinhLuanService : IBaseService<BinhLuanModel>
{
    public Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int soSao, int pageNumber, int limit);

    public Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangNguoiDungAsync(Guid userId, int pageNumber, int limit);
    public Task ThemAsync(Guid sp_id, string noiDung, List<IFormFile>? listImages, ClaimsPrincipal userNowFromJwt);
}