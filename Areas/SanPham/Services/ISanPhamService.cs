using System.Security.Claims;
using App.Areas.BinhLuan.Models;
using App.Areas.DoanhNghiep.Models;
using App.Areas.DTO;
using App.Areas.SanPham.Models;
using App.Services;

namespace App.Areas.SanPham.Services;

public interface ISanPhamService : IBaseService<SanPhamModel>
{
    public Task<SanPhamModel> LayMotBangMaTruyXuatAsync(string traceCode);
    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDanhMucAsync(Guid dm_id, int pageNumber, int limit, string search, bool descending);
    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSoHuuAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);
    public Task<int> LayTongSoBangDoanhNghiepSoHuuAsync(Guid dn_id);
    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepVanTaiAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);
    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSanXuatAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);
    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangNhaMayAsync(Guid nm_id, int pageNumber, int limit, string search, bool descending);
    public Task TaiLenAnhSanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);
    public Task XoaAnhSanPhamAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt);
    public Task ThemSaoAsync(Guid id, int soSao, ClaimsPrincipal userNowFromJWT);
    public Task<double> LaySoSaoAsync(Guid id);
    public Task<int> LaySoSaoCuaMotUserAsync(Guid id, Guid userId);
    public Task<Guid?> LayDoanhNghiepIdSoHuuSanPhamAsync(Guid id);
}