using App.Areas.SuKienTruyXuat.Models;
using App.Database;

namespace App.Areas.SuKienTruyXuat.Repositories;

public interface ISuKienTruyXuatRepository : IBaseRepository<SuKienTruyXuatModel>
{
    public Task<List<SuKienTruyXuatModel>> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending);
    public Task<int> LayTongSoBangSanPhamAsync(Guid sp_id);
    public Task<bool> KiemTraTonTaiBangMaSuKienAsync(string sk_MaSK, Guid? id = null);
    public Task<SuKienTruyXuatModel> LayMotBangMaSuKienAsync(string sk_MaSK);
}