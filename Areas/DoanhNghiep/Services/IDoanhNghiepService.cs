using System.Security.Claims;
using App.Areas.DTO;
using App.Areas.DoanhNghiep.Models;
using App.Services;

namespace App.Areas.DoanhNghiep.Services;

public interface IDoanhNghiepService : IBaseService<DoanhNghiepModel>
{
    public Task<DoanhNghiepModel> LayMotBangMaSoThueAsync(string dn_MaSoThue);
    public Task ThemSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt);
    public Task TuBoSoHuuDoanhNghiepAsync(Guid id, ClaimsPrincipal userNowFromJwt);
    public Task XoaSoHuuDoanhNghiepAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt);
    public Task PhanQuyenDoanhNghiepAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt);
    public Task PhanQuyenSanPhamTheoDoanhNghiepAsync(Guid id, PhanQuyenDTO phanQuyenDTO, ClaimsPrincipal userNowFromJwt);
}