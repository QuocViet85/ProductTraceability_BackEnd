using System.Security.Claims;
using App.Areas.DoanhNghiep.Models;
using App.Services;

namespace App.Areas.DoanhNghiep.Services;

public interface IDoanhNghiepService : IBaseService<DoanhNghiepModel>
{
    public Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string maSoThue);
    public Task ThemSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt);
    public Task TuBoSoHuuDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt);
    public Task XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId);
}