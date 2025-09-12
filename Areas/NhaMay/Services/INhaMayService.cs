using System.Security.Claims;
using App.Areas.DTO;
using App.Areas.NhaMay.Models;
using App.Services;

namespace App.Areas.NhaMay.Services;

public interface INhaMayService : IBaseService<NhaMayModel>
{
    public Task<NhaMayModel> LayMotBangMaNhaMayAsync(string nm_MaNM);
    public Task ThemDoanhNghiepVaoNhaMayAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);
    public Task XoaDoanhNghiepKhoiNhaMayAsync(Guid id, ClaimsPrincipal userNowFromJwt);
    public Task PhanQuyenNhaMayAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt);
    public Task TaiLenAnhNhaMayAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);
    public Task XoaAnhNhaMayAsync(Guid id, Guid f_id, ClaimsPrincipal userNowFromJwt);
}   