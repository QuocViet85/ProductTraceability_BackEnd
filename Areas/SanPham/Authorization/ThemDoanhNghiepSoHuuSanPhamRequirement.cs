using Microsoft.AspNetCore.Authorization;

namespace App.Areas.SanPham.Authorization;

public class ThemDoanhNghiepSoHuuSanPhamRequirement : IAuthorizationRequirement
{
    public Guid DN_Id { set; get; }

    public ThemDoanhNghiepSoHuuSanPhamRequirement(Guid dn_id)
    {
        DN_Id = dn_id;
    }
}