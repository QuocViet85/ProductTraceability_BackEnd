using App.Areas.DoanhNghiep.Models;
using App.Database;
namespace App.Areas.DoanhNghiep.Repositories;

public interface IDoanhNghiepRepository : IBaseRepository<DoanhNghiepModel>
{
    public Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string maSoThue);
    public Task<bool> KiemTraTonTaiBangMaSoThueAsync(string maSoThue, Guid? id = null); 
    public Task<bool> KiemTraTonTaiBangMaGLNAsync(string maGLN, Guid? id = null);
    public Task<bool> KiemTraLaChuDoanhNghiepAsync(Guid id, Guid userId);
    public Task<int> ThemSoHuuDoanhNghiepAsync(ChuDoanhNghiepModel chuDoanhNghiep);
    public Task<int> TuBoSoHuuDoanhNghiepAsync(Guid id, Guid userId);
    public Task<int> XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId);
}