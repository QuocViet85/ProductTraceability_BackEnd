using App.Areas.BinhLuan.Models;
using App.Database;

namespace App.Areas.BinhLuan.Repositories;

public interface IBinhLuanRepository : IBaseRepository<BinhLuanModel>
{
    public Task<List<BinhLuanModel>> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit);
    public Task<int> LayTongSoBangSanPhamAsync(Guid sp_Id);
    public Task<int> ThemAsync(BinhLuanModel binhLuan);
    public Task<int> XoaAsync(BinhLuanModel binhLuan);
}