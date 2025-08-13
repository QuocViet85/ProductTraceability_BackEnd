using App.Areas.LoSanPham.Models;
using App.Services;

namespace App.Areas.LoSanPham.Services;

public interface ILoSanPhamService : IBaseService<LoSanPhamModel>
{
    public Task<(int totalItems, List<LoSanPhamModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit, string search, bool descending);

    public Task<LoSanPhamModel> LayMotBangMaLoSanPhamAsync(string lsp_MaLSP);
}