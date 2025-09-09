using App.Areas.BinhLuan.Models;
using App.Database;

namespace App.Areas.BinhLuan.Repositories;

public interface IBinhLuanRepository : IBaseRepository<BinhLuanModel>
{
    public Task<List<BinhLuanModel>> LayNhieuBangTaiNguyenAsync(string kieuTaiNguyen, Guid taiNguyenId, int pageNumber, int limit);
    public Task<int> LayTongSoBangTaiNguyenAsync(string kieuTaiNguyen, Guid sp_Id);
    public Task<int> ThemAsync(BinhLuanModel binhLuan);
    public Task<int> XoaAsync(BinhLuanModel binhLuan);
}