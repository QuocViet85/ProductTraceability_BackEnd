using App.Areas.SuKienTruyXuat.Models;
using App.Database;

namespace App.Areas.SuKienTruyXuat.Repositories;

public interface ISuKienTruyXuatRepository : IBaseRepository<SuKienTruyXuatModel>
{
    public Task<List<SuKienTruyXuatModel>> LayNhieuBangLoSanPhamAsync(Guid lsp_Id, int pageNumber, int limit, string search, bool descending);
    public Task<int> LayTongSoBangLoSanPhamAsync(Guid lsp_Id);
    public Task<bool> KiemTraTonTaiBangMaSuKienAsync(string sk_MaSK, Guid? id = null);
    public Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK);
}