using System.Security.Claims;
using App.Areas.DTO;
using App.Areas.SanPham.Models;
using App.Services;

namespace App.Areas.SanPham.Services;

public interface ISanPhamService : IBaseService<SanPhamModel>
{
    public Task<SanPhamModel> LayMotBangMaTruyXuatAsync(string traceCode);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDanhMucAsync(Guid dm_id, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSoHuuAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepVanTaiAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangDoanhNghiepSanXuatAsync(Guid dn_id, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangNguoiPhuTrachAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalItems, List<SanPhamModel> listItems)> LayNhieuBangNhaMayAsync(Guid nm_id, int pageNumber, int limit, string search, bool descending);

    public Task DoiDoanhNghiepSoHuuSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt);

    public Task ThemDoanhNghiepVanTaiSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt);

    public Task XoaDoanhNghiepVanTaiSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task ThemDoanhNghiepSanXuatSanPhamAsync(Guid id, Guid dn_id, ClaimsPrincipal userNowFromJwt);

    public Task XoaDoanhNghiepSanXuatSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task ThemNguoiPhuTrachSanPhamAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt);

    public Task XoaNguoiPhuTrachSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task ThemNhaMayCuaSanPhamAsync(Guid id, Guid nm_id, ClaimsPrincipal userNowFromJwt);

    public Task XoaNhaMayCuaSanPhamAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task TaiLenAnhSanPhamAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);

    public Task XoaAnhSanPhamAsync(Guid id, Guid fileId, ClaimsPrincipal userNowFromJwt);

    public Task ThemSaoAsync(Guid id, int soSao, ClaimsPrincipal userNowFromJWT);

    public Task<double> LaySoSaoAsync(Guid id);

}