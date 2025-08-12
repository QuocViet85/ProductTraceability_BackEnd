using System.Security.Claims;
using App.Areas.NhaMay.Models;
using App.Services;

namespace App.Areas.NhaMay.Services;

public interface INhaMayService : IBaseService<NhaMayModel>
{
    public Task<NhaMayModel> LayMotBangMaNhaMayAsync(string maNhaMay);
    public Task ThemDoanhNghiepVaoNhaMayAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);
    public Task XoaDoanhNghiepKhoiNhaMayAsync(Guid id, ClaimsPrincipal userNowFromJwt);

}