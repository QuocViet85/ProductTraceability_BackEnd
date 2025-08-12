using Microsoft.AspNetCore.Authorization;

namespace App.Areas.DoanhNghiep.Auth;

public class SuaXoaDoanhNghiepRequirement : IAuthorizationRequirement
{
    public bool Xoa { set; get; }

    public SuaXoaDoanhNghiepRequirement(bool xoa = false)
    {
        Xoa = xoa;
    }
}