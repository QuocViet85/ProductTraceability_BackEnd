using App.Areas.DanhMuc.Models;
using App.Database;

namespace App.Areas.DanhMuc.Repositories;

public interface IDanhMucRepository : IBaseRepository<DanhMucModel>
{
    public Task<bool> KiemTraTonTaiBangTenAsync(string name, Guid? id = null);
    public Task<DanhMucModel> LayMotBangTenAsync(string name);
}