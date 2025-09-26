using App.Areas.NhaMay.Models;
using App.Database;

namespace App.Areas.NhaMay.Repositories;

public interface INhaMayRepository : IBaseRepository<NhaMayModel>
{
    public Task<List<NhaMayCoBanModel>> LayNhieuCoBanAsync(int pageNumber, int limit, string search, bool descending);
    public Task<bool> KiemTraTonTaiBangMaNhaMayAsync(string nm_MaNM, Guid? id = null);
    public Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM);
    public Task<int> XoaPhanQuyenNhaMayAsync(Guid id, Guid? userId = null);
}