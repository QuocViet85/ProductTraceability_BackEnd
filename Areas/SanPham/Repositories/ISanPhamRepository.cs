using App.Areas.SanPham.Models;
using App.Database;

namespace App.Areas.SanPham.Repositories;

public interface ISanPhamRepository : IBaseRepository<SanPhamModel>
{
    public Task<SanPhamModel> LayMotBangMaTruyXuatAsync(string sp_MaTruyXuat);

    public Task<bool> KiemTraTonTaiBangMaTruyXuatAsync(string sp_MaTruyXuat, Guid? id = null);

    public Task<bool> KiemTraTonTaiBangMaVachAsync(string sp_MaVach, Guid? id = null);

    public Task<List<SanPhamModel>> LayNhieuBangDanhMucAsync(Guid dm_id, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangDanhMucAsync(Guid dm_id);

    public Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepSoHuuAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangDoanhNghiepSoHuuAsync(Guid dn_id);

    public Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepVanTaiAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangDoanhNghiepVanTaiAsync(Guid dn_id);

    public Task<List<SanPhamModel>> LayNhieuBangDoanhNghiepSanXuatAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangDoanhNghiepSanXuatAsync(Guid dn_id);

    public Task<List<SanPhamModel>> LayNhieuBangNguoiPhuTrachAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangNguoiPhuTrachAsync(Guid userId);

    public Task<List<SanPhamModel>> LayNhieuBangNhaMayAsync(Guid nm_id, int pageNumber, int limit, string search, bool descending);

    public Task<int> LayTongSoBangNhaMayAsync(Guid nm_id);

    public Task<int> ThemSaoAsync(SaoSanPhamModel saoSanPham);

    public Task<double> LaySoSaoAsync(Guid id);

    public Task<int> LaySoSaoCuaMotUserAsync(Guid id, Guid userId);

    public Task<Guid?> LayDoanhNghiepIdSoHuuSanPhamAsync(Guid id);
}


/*
Có api lấy Product từ tài nguyên khác vì:
- Product trong các tài nguyên khác có thể rất nhiều nên cần limit, search. 
- Người dùng cần lấy product từ 1 tài nguyên khác. 

*/