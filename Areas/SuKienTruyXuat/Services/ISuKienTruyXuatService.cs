using System.Security.Claims;
using App.Areas.SuKienTruyXuat.Models;
using App.Services;

namespace App.Areas.SuKienTruyXuat.Services;

public interface ISuKienTruyXuatService : IBaseService<SuKienTruyXuatModel>
{
    public Task<(int totalItems, List<SuKienTruyXuatModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending);
    public Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK);
    public Task TaiLenAnhSuKienAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);
    public Task XoaAnhSuKienAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt);
}