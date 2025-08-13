using App.Areas.LoSanPham.Models;
using App.Database;

namespace App.Areas.LoSanPham.Repositories;

public interface ILoSanPhamRepository : IBaseRepository<LoSanPhamModel>
{
    public Task<List<LoSanPhamModel>> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit, string search, bool descending);
    public Task<int> LayTongSoBangSanPhamAsync(Guid sp_Id);
    public Task<LoSanPhamModel> LayMotBangMaLoSanPhamAsync(string lsp_MaLSP);
    public Task<bool> KiemTraTonTaiBangMaLoSanPhamAsync(string lsp_MaLSP, Guid? id = null);
}