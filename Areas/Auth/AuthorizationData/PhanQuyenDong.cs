using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage;

namespace App.Areas.Auth.AuthorizationData;

public static class PhanQuyenDong
{
    public static Claim TaoClaimPhanQuyenDong(string kieuTaiNguyen, Guid taiNguyenId, string quyen)
    {
        return new Claim("quyen", TaoGiaTriClaimPhanQuyenDong(kieuTaiNguyen, taiNguyenId, quyen));
    }

    public static string TaoGiaTriClaimPhanQuyenDong(string kieuTaiNguyen, Guid taiNguyenId, string quyen)
    {
        return $"{kieuTaiNguyen}.{quyen}.{taiNguyenId}";
    }
}

public static class Quyen
{
    public const string ADMIN = "admin";
    public const string THEM = "them";
    public const string SUA = "sua";
    public const string XOA = "xoa";
}

/*  

*/