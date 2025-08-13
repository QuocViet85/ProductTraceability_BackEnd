using App.Areas.BinhLuan.Models;
using App.Services;

namespace App.Areas.BinhLuan.Services;

public interface IBinhLuanService : IBaseService<BinhLuanModel>
{
    public Task<(int totalItems, List<BinhLuanModel> listItems)> LayNhieuBangSanPhamAsync(Guid sp_Id, int pageNumber, int limit);
}