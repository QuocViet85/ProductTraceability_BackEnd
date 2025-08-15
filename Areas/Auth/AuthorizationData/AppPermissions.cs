namespace App.Areas.Auth.AuthorizationData;

public static class AppPermissions
{
    public const string Permissions = "permissions";
    public const string DN_Admin = "dn.admin";
    public const string DN_Sua = "dn.sua";
    public const string DN_Xoa = "dn.xoa";
    public const string SP_DN_Admin = "sp.dn.admin";
    public const string SP_DN_Them = "sp.dn.them";
    public const string SP_DN_Sua = "sp.dn.sua";
    public const string SP_DN_Xoa = "sp.dn.xoa";
    public const string NM_Admin = "nm.admin";
    public const string NM_Sua = "nm.sua";
    public const string NM_Xoa = "nm.xoa";

    public static string TaoGiaTriPhanQuyen(string permission, Guid id)
    {
        return $"{permission}.{id}";
    }
}



/*  

*/