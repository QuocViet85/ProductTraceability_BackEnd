using App.Areas.BaiViet.Model;
using App.Database;

namespace App.Areas.BaiViet.Repositories;

public interface IBaiVietRepository : IBaseRepository<BaiVietModel>
{
    public Task<List<BaiVietModel>> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit, string search, bool descending);
    public Task<int> LayTongSoBangSanPhamAsync(Guid sp_id);
}