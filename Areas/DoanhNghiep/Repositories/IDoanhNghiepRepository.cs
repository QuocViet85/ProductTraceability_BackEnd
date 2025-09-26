using App.Areas.DoanhNghiep.Models;
using App.Database;
namespace App.Areas.DoanhNghiep.Repositories;

public interface IDoanhNghiepRepository : IBaseRepository<DoanhNghiepModel>
{
    public Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string dn_MaSoThue);
    public Task<List<DoanhNghiepCoBanModel>> LayNhieuCoBanAsync(int pageNumber, int limit, string search, bool descending);
    public Task<bool> KiemTraTonTaiBangMaSoThueAsync(string dn_MaSoThue, Guid? id = null);
    public Task<bool> KiemTraTonTaiBangMaGLNAsync(string dn_MaGLN, Guid? id = null);
    public Task<bool> KiemTraLaChuDoanhNghiepAsync(Guid id, Guid userId);
    public Task<int> ThemSoHuuDoanhNghiepAsync(ChuDoanhNghiepModel chuDoanhNghiep);
    public Task<int> TuBoSoHuuDoanhNghiepAsync(Guid id, Guid userId);
    public Task<int> XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId);
    public Task<int> XoaPhanQuyenDoanhNghiepAsync(Guid id, Guid? userId = null);
    public Task<int> XoaPhanQuyenSanPhamTheoDoanhNghiepAsync(Guid id, Guid? userId = null);
    public Task<bool> KiemTraDangTheoDoiDoanhNghiepAsync(Guid dn_id, Guid userId);
    public Task<int> ThemTheoDoiDoanhNghiepAsync(TheoDoiDoanhNghiepModel theoDoiDoanhNghiep);
    public Task<int> HuyTheoDoiDoanhNghiepAsync(Guid dn_id, Guid userId);
    public Task<int> LaySoTheoDoiAsync(Guid dn_id);
}