using App.Areas.DanhMuc.Models;
using App.Services;

namespace App.Areas.DanhMuc.Services;

public interface IDanhMucService : IBaseService<DanhMucModel>
{
    public Task<DanhMucModel> LayMotBangTenAsync(string name);
}