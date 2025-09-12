using System.Security.Claims;
using App.Areas.LoSanPham.Models;
using App.Services;

namespace App.Areas.LoSanPham.Services;

public interface ILoSanPhamService : IBaseService<LoSanPhamModel>
{
    public Task<(int totalItems, List<LoSanPhamModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit, string search, bool descending);

    public Task<LoSanPhamModel> LayMotBangMaLoSanPhamAsync(string lsp_MaLSP);

    public Task TaiLenAnhLoSanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);

    public Task XoaAnhLoSanPhamAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt);
}