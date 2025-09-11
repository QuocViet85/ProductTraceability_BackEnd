using App.Areas.BinhLuan.Models;
using App.Database;

namespace App.Areas.BinhLuan.Repositories;

public interface IBinhLuanRepository : IBaseRepository<BinhLuanModel>
{
    public Task<List<BinhLuanModel>> LayNhieuBangSanPhamAsync(Guid sp_id, int pageNumber, int limit);
    public Task<int> LayTongSoBangSanPhamAsync(Guid sp_id);
    public Task<List<BinhLuanModel>> LayNhieuBangNguoiDungAsync(Guid userId, int pageNumber, int limit);
    public Task<int> LayTongSoBangNguoiDungAsync(Guid userId);
    public Task<int> ThemAsync(BinhLuanModel binhLuan);
    public Task<int> XoaAsync(BinhLuanModel binhLuan);
}