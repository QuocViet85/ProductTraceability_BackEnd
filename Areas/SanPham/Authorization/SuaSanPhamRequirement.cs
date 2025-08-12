using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.SanPham.Authorization;

public class SuaSanPhamRequirement : IAuthorizationRequirement
{
    public string SP_MaTruyXuat { set; get; }

    public SuaSanPhamRequirement(string sp_MaTruyXuat)
    {
        SP_MaTruyXuat = sp_MaTruyXuat;
    }
}