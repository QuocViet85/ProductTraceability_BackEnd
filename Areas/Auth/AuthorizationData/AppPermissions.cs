using System.Security.Claims;

namespace App.Areas.Auth.AuthorizationData;

public static class AppPermissions
{
    public const string Permissions = "permissions";
    public const string DN_Admin = "dn.admin";
    public const string DN_Sua = "dn.sua";
    public const string DN_Xoa = "dn.xoa";
    public const string SP_Admin = "sp.admin";
    public const string SP_Them = "sp.them";
    public const string SP_Sua = "sp.sua";
    public const string SP_Xoa = "sp.xoa";
    public const string NM_Admin = "nm.admin";
    public const string NM_Them = "nm.them";
    public const string NM_Sua = "nm.sua";
    public const string NM_Xoa = "nm.xoa";

    public static string PhanQuyenBangDNId(string permission, Guid dn_id)
    {
        return $"{permission}.{dn_id}";
    }
}



/*  

*/